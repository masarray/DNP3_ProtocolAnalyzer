using DNP3_ProtocolAnalyzer.Core.Abstractions;
using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Protocols.Dnp3.Decoding;

public sealed class Dnp3FrameDecoder : IFrameDecoder
{
    public DecodedFrame Decode(PacketRecord packet)
    {
        var parsed = Dnp3PayloadParser.Parse(packet.Payload);
        bool unsolicitedHint = IsUnsolicited(parsed);

        return new DecodedFrame
        {
            TimestampText = packet.TimestampUtc.ToLocalTime().ToString("HH:mm:ss.fff"),
            Direction = packet.Direction,
            Source = FormatEndpoint(parsed.Source),
            Destination = FormatEndpoint(parsed.Destination),
            Transport = FormatTransport(parsed.ApplicationControl),
            ApplicationControl = FormatApplicationControl(parsed.ApplicationControl),
            FunctionCode = Dnp3FunctionCodeFormatter.Format(parsed.FunctionCode, unsolicitedHint),
            ObjectSummary = FormatObjectSummary(parsed),
            IinSummary = FormatIin(parsed.Iin),
            SemanticTag = BuildSemanticTag(parsed, packet.Summary),
            RawHex = BitConverter.ToString(packet.Payload).Replace("-", " "),
            Summary = BuildSummary(parsed, packet.Summary)
        };
    }

    private static bool IsUnsolicited(Dnp3ParsedFrame parsed)
    {
        return parsed.FunctionCode == 0x82
            || (parsed.FunctionCode == 0x81 && parsed.ApplicationControl.HasValue && (parsed.ApplicationControl.Value & 0x20) != 0);
    }

    private static string FormatEndpoint(ushort address)
    {
        return address == 0 ? "Addr ?" : $"Addr {address}";
    }

    private static string FormatTransport(byte? appControl)
    {
        if (!appControl.HasValue)
        {
            return "Transport n/a";
        }

        bool fir = (appControl.Value & 0x80) != 0;
        bool fin = (appControl.Value & 0x40) != 0;
        int seq = appControl.Value & 0x0F;
        return $"{(fir ? "FIR" : "-")}/{(fin ? "FIN" : "-")} Seq {seq}";
    }

    private static string FormatApplicationControl(byte? appControl)
    {
        if (!appControl.HasValue)
        {
            return "App Ctrl n/a";
        }

        bool fir = (appControl.Value & 0x80) != 0;
        bool fin = (appControl.Value & 0x40) != 0;
        bool con = (appControl.Value & 0x20) != 0;
        bool uns = (appControl.Value & 0x10) != 0;
        int seq = appControl.Value & 0x0F;
        return $"{(fir ? "FIR" : "-")} | {(fin ? "FIN" : "-")} | {(con ? "CON" : "-")} | {(uns ? "UNS" : "-")} | SEQ {seq}";
    }

    private static string FormatObjectSummary(Dnp3ParsedFrame parsed)
    {
        if (!parsed.ObjectGroup.HasValue || !parsed.ObjectVariation.HasValue)
        {
            return "Object n/a";
        }

        string qualifierText = parsed.Qualifier.HasValue ? $" Q{parsed.Qualifier.Value:X2}" : string.Empty;
        return $"G{parsed.ObjectGroup.Value}V{parsed.ObjectVariation.Value}{qualifierText}";
    }

    private static string FormatIin(ushort? iin)
    {
        if (!iin.HasValue)
        {
            return "n/a";
        }

        if (iin.Value == 0)
        {
            return "IIN clear";
        }

        var labels = new List<string>();
        if ((iin.Value & 0x0080) != 0) labels.Add("DEVICE_RESTART");
        if ((iin.Value & 0x0002) != 0) labels.Add("CLASS1_EVENTS");
        if ((iin.Value & 0x0004) != 0) labels.Add("CLASS2_EVENTS");
        if ((iin.Value & 0x0008) != 0) labels.Add("CLASS3_EVENTS");
        if ((iin.Value & 0x2000) != 0) labels.Add("NEED_TIME");
        return labels.Count == 0 ? $"IIN 0x{iin.Value:X4}" : string.Join(", ", labels);
    }

    private static string BuildSemanticTag(Dnp3ParsedFrame parsed, string fallbackSummary)
    {
        return Dnp3FunctionCodeFormatter.Format(parsed.FunctionCode, IsUnsolicited(parsed)) switch
        {
            "READ" => "Integrity / Polling",
            "RESPONSE" => "Solicited Response",
            "UNSOLICITED_RESPONSE" => "SOE / Event",
            "ENABLE_UNSOLICITED" => "Startup Conditioning",
            "OPERATE" => "Control Lifecycle",
            "CONFIRM" => "Confirm / Ack",
            _ => fallbackSummary.Contains("time", StringComparison.OrdinalIgnoreCase) ? "Time Sync" : "General DNP3"
        };
    }

    private static string BuildSummary(Dnp3ParsedFrame parsed, string fallbackSummary)
    {
        string function = Dnp3FunctionCodeFormatter.Format(parsed.FunctionCode, IsUnsolicited(parsed));
        string objectText = FormatObjectSummary(parsed);
        return parsed.HasLinkHeader
            ? $"{function} {objectText} | Src {parsed.Source} Dst {parsed.Destination} | {parsed.DecodeNote}"
            : fallbackSummary;
    }
}
