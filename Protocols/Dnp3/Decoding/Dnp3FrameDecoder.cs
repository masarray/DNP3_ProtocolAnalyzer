using DNP3_ProtocolAnalyzer.Core.Abstractions;
using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Protocols.Dnp3.Decoding;

public sealed class Dnp3FrameDecoder : IFrameDecoder
{
    public DecodedFrame Decode(PacketRecord packet)
    {
        var summary = packet.Summary;
        return new DecodedFrame
        {
            TimestampText = packet.TimestampUtc.ToLocalTime().ToString("HH:mm:ss.fff"),
            Direction = packet.Direction,
            Source = packet.Direction == "TX" ? "Master(1)" : "Relay(100)",
            Destination = packet.Direction == "TX" ? "Relay(100)" : "Master(1)",
            Transport = GuessTransport(summary),
            ApplicationControl = GuessApplicationControl(summary),
            FunctionCode = GuessFunctionCode(summary),
            ObjectSummary = GuessObjectSummary(summary),
            IinSummary = GuessIin(summary),
            SemanticTag = GuessSemanticTag(summary),
            RawHex = BitConverter.ToString(packet.Payload).Replace("-", " "),
            Summary = summary
        };
    }

    private static string GuessTransport(string summary)
    {
        if (summary.Contains("unsolicited", StringComparison.OrdinalIgnoreCase)) return "FIR/FIN Seq 2";
        if (summary.Contains("integrity", StringComparison.OrdinalIgnoreCase)) return "FIR/FIN Seq 1";
        if (summary.Contains("confirm", StringComparison.OrdinalIgnoreCase)) return "FIR/FIN Seq 0";
        return "Single Fragment";
    }

    private static string GuessApplicationControl(string summary)
    {
        if (summary.Contains("unsolicited", StringComparison.OrdinalIgnoreCase)) return "UNS | CON";
        if (summary.Contains("response", StringComparison.OrdinalIgnoreCase)) return "FIR | FIN | CON";
        if (summary.Contains("enable", StringComparison.OrdinalIgnoreCase)) return "FIR | FIN";
        return "FIR | FIN | SEQ";
    }

    private static string GuessFunctionCode(string summary)
    {
        if (summary.Contains("integrity poll request", StringComparison.OrdinalIgnoreCase)) return "READ";
        if (summary.Contains("integrity poll response", StringComparison.OrdinalIgnoreCase)) return "RESPONSE";
        if (summary.Contains("unsolicited", StringComparison.OrdinalIgnoreCase)) return "UNSOLICITED_RESPONSE";
        if (summary.Contains("enable unsolicited", StringComparison.OrdinalIgnoreCase)) return "ENABLE_UNSOLICITED";
        if (summary.Contains("confirm", StringComparison.OrdinalIgnoreCase)) return "CONFIRM";
        if (summary.Contains("operate", StringComparison.OrdinalIgnoreCase)) return "OPERATE";
        return "APPLICATION_FRAGMENT";
    }

    private static string GuessObjectSummary(string summary)
    {
        if (summary.Contains("binary event", StringComparison.OrdinalIgnoreCase)) return "G2V2 Binary Event";
        if (summary.Contains("binary input", StringComparison.OrdinalIgnoreCase)) return "G1V2 Binary Input";
        if (summary.Contains("analog", StringComparison.OrdinalIgnoreCase)) return "G30V2 Analog Input";
        if (summary.Contains("classes 1/2/3", StringComparison.OrdinalIgnoreCase)) return "G60V2/G60V3/G60V4 Class Enable";
        if (summary.Contains("class 0", StringComparison.OrdinalIgnoreCase)) return "G60V1 Class 0 Request";
        if (summary.Contains("crob", StringComparison.OrdinalIgnoreCase)) return "G12V1 CROB";
        return "Object mapping pending";
    }

    private static string GuessIin(string summary)
    {
        if (summary.Contains("unsolicited", StringComparison.OrdinalIgnoreCase)) return "IIN1.7 DEVICE_RESTART";
        if (summary.Contains("response", StringComparison.OrdinalIgnoreCase)) return "IIN clear";
        return "n/a";
    }

    private static string GuessSemanticTag(string summary)
    {
        if (summary.Contains("integrity", StringComparison.OrdinalIgnoreCase)) return "Integrity Poll";
        if (summary.Contains("unsolicited", StringComparison.OrdinalIgnoreCase)) return "SOE / Event";
        if (summary.Contains("operate", StringComparison.OrdinalIgnoreCase)) return "Control Lifecycle";
        if (summary.Contains("enable", StringComparison.OrdinalIgnoreCase)) return "Startup Conditioning";
        return "General DNP3";
    }
}
