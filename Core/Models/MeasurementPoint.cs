namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class MeasurementPoint
{
    public string TimestampText { get; set; } = string.Empty;
    public string PointAddress { get; set; } = string.Empty;
    public string PointType { get; set; } = string.Empty;
    public string GroupVariation { get; set; } = string.Empty;
    public string ValueText { get; set; } = string.Empty;
    public string Quality { get; set; } = string.Empty;
    public string EventClass { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Freshness { get; set; } = string.Empty;
}
