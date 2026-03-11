using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Testing.Dnp3;

public static class Dnp3ScenarioCatalog
{
    public static IReadOnlyList<TestScenario> Default { get; } =
    [
        new TestScenario
        {
            Name = "Integrity Poll",
            Goal = "Verify Class 0 data acquisition and object decoding.",
            ExpectedOutcome = "Analyzer correlates request and response and updates point values."
        },
        new TestScenario
        {
            Name = "Class Poll",
            Goal = "Validate event class sequencing and event backlog behavior.",
            ExpectedOutcome = "Analyzer identifies Class 1, 2, and 3 result boundaries."
        },
        new TestScenario
        {
            Name = "Select Operate",
            Goal = "Track command lifecycle for CROB operations.",
            ExpectedOutcome = "Analyzer reports select-before-operate timing and confirmations."
        },
        new TestScenario
        {
            Name = "Time Sync",
            Goal = "Check time synchronization command flow.",
            ExpectedOutcome = "Analyzer highlights skew or missing acknowledgements."
        }
    ];
}
