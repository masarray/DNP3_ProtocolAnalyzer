namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class StatusEntry
{
    public string Severity { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TimestampText { get; set; } = string.Empty;
}
