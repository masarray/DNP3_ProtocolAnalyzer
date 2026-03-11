namespace DNP3_ProtocolAnalyzer.Protocols.Dnp3.Decoding;

internal sealed class Dnp3ParsedFrame
{
    public bool IsValidStart { get; init; }
    public bool HasLinkHeader { get; init; }
    public int Length { get; init; }
    public byte Control { get; init; }
    public ushort Destination { get; init; }
    public ushort Source { get; init; }
    public byte? ApplicationControl { get; init; }
    public byte? FunctionCode { get; init; }
    public ushort? Iin { get; init; }
    public byte? ObjectGroup { get; init; }
    public byte? ObjectVariation { get; init; }
    public byte? Qualifier { get; init; }
    public string DecodeNote { get; init; } = string.Empty;
}
