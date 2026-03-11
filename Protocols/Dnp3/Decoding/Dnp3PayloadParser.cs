namespace DNP3_ProtocolAnalyzer.Protocols.Dnp3.Decoding;

internal static class Dnp3PayloadParser
{
    public static Dnp3ParsedFrame Parse(byte[] payload)
    {
        if (payload.Length < 8)
        {
            return new Dnp3ParsedFrame
            {
                DecodeNote = "Payload too short for DNP3 link header."
            };
        }

        bool validStart = payload[0] == 0x05 && payload[1] == 0x64;
        ushort destination = (ushort)(payload[4] | (payload[5] << 8));
        ushort source = (ushort)(payload[6] | (payload[7] << 8));

        byte? appControl = payload.Length > 8 ? payload[8] : null;
        byte? functionCode = payload.Length > 9 ? payload[9] : null;
        ushort? iin = null;
        int objectOffset = 10;

        if (functionCode is 0x81 or 0x82 && payload.Length >= 12)
        {
            iin = (ushort)(payload[10] | (payload[11] << 8));
            objectOffset = 12;
        }

        byte? objectGroup = payload.Length > objectOffset ? payload[objectOffset] : null;
        byte? objectVariation = payload.Length > objectOffset + 1 ? payload[objectOffset + 1] : null;
        byte? qualifier = payload.Length > objectOffset + 2 ? payload[objectOffset + 2] : null;

        return new Dnp3ParsedFrame
        {
            IsValidStart = validStart,
            HasLinkHeader = payload.Length >= 8,
            Length = payload[2],
            Control = payload[3],
            Destination = destination,
            Source = source,
            ApplicationControl = appControl,
            FunctionCode = functionCode,
            Iin = iin,
            ObjectGroup = objectGroup,
            ObjectVariation = objectVariation,
            Qualifier = qualifier,
            DecodeNote = validStart ? "Parsed from DNP3 payload bytes." : "Start bytes invalid for DNP3."
        };
    }
}
