namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class ProtocolEvent
{
    public string TimestampText { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string PointAddress { get; set; } = string.Empty;
    public string Verdict { get; set; } = string.Empty;
    public string EventClass { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
