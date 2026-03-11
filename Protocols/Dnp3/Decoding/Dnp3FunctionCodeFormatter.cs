namespace DNP3_ProtocolAnalyzer.Protocols.Dnp3.Decoding;

internal static class Dnp3FunctionCodeFormatter
{
    public static string Format(byte? functionCode, bool unsolicitedHint)
    {
        if (!functionCode.HasValue)
        {
            return "UNKNOWN";
        }

        return functionCode.Value switch
        {
            0x00 => "CONFIRM",
            0x01 => "READ",
            0x02 => "WRITE",
            0x03 => "SELECT",
            0x04 => "OPERATE",
            0x05 => "DIRECT_OPERATE",
            0x14 => "ENABLE_UNSOLICITED",
            0x15 => "DISABLE_UNSOLICITED",
            0x81 when unsolicitedHint => "UNSOLICITED_RESPONSE",
            0x81 => "RESPONSE",
            0x82 => "UNSOLICITED_RESPONSE",
            _ => $"FC 0x{functionCode.Value:X2}"
        };
    }
}
