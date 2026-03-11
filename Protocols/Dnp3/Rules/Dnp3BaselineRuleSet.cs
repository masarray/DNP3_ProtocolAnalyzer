using DNP3_ProtocolAnalyzer.Core.Abstractions;
using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Protocols.Dnp3.Rules;

public sealed class Dnp3BaselineRuleSet : IProtocolRuleSet
{
    public IReadOnlyList<ValidationIssue> Evaluate(IReadOnlyList<DecodedFrame> frames, IReadOnlyList<MeasurementPoint> points)
    {
        var issues = new List<ValidationIssue>();

        if (!frames.Any(frame => frame.FunctionCode == "ENABLE_UNSOLICITED"))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "WARN",
                Title = "Startup Conditioning",
                Detail = "Session did not show enable unsolicited negotiation before event traffic."
            });
        }

        if (!frames.Any(frame => frame.FunctionCode == "UNSOLICITED_RESPONSE"))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "WARN",
                Title = "Unsolicited",
                Detail = "No unsolicited response observed in the current session."
            });
        }

        if (!frames.Any(frame => frame.FunctionCode == "CONFIRM"))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "WARN",
                Title = "Confirm",
                Detail = "Analyzer did not observe confirm traffic after unsolicited/event transfer."
            });
        }

        if (!points.Any(point => point.PointType == "Analog Input"))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "INFO",
                Title = "Analog Mapping",
                Detail = "Analog input projection has not been populated yet."
            });
        }

        if (issues.Count == 0)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "PASS",
                Title = "Baseline",
                Detail = "Sample session satisfied baseline DNP3 relay workflow expectations."
            });
        }

        return issues;
    }
}
