using System.Collections.Generic;
using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Core.Abstractions;

public interface IProtocolRuleSet
{
    IReadOnlyList<ValidationIssue> Evaluate(IReadOnlyList<DecodedFrame> frames, IReadOnlyList<MeasurementPoint> points);
}
