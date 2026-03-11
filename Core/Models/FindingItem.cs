namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class FindingItem
{
    public string TimestampText { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}
