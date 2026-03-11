namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class DecodedFrame
{
    public string TimestampText { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Transport { get; set; } = string.Empty;
    public string ApplicationControl { get; set; } = string.Empty;
    public string FunctionCode { get; set; } = string.Empty;
    public string ObjectSummary { get; set; } = string.Empty;
    public string IinSummary { get; set; } = string.Empty;
    public string SemanticTag { get; set; } = string.Empty;
    public string RawHex { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}
