namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class TestScenario
{
    public string Name { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public string ExpectedOutcome { get; set; } = string.Empty;
}
