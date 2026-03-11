namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class ValidationIssue
{
    public string Severity { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}
