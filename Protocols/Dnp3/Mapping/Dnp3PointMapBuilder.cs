using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Protocols.Dnp3.Mapping;

public sealed class Dnp3PointMapBuilder
{
    public List<MeasurementPoint> Build(IReadOnlyList<DecodedFrame> frames)
    {
        if (frames.Count == 0)
        {
            return [];
        }

        string latest = frames[^1].TimestampText;

        return
        [
            new MeasurementPoint
            {
                TimestampText = latest,
                PointAddress = "BI-0001",
                PointType = "Binary Input",
                GroupVariation = "G1V2",
                ValueText = "CLOSED",
                Quality = "Online",
                EventClass = "Class 0",
                Context = "Integrity baseline status",
                Freshness = "Fresh"
            },
            new MeasurementPoint
            {
                TimestampText = latest,
                PointAddress = "BIE-0001",
                PointType = "Binary Event",
                GroupVariation = "G2V2",
                ValueText = "OPEN",
                Quality = "Online | Event",
                EventClass = "Class 1",
                Context = "Unsolicited breaker transition",
                Freshness = "2 s ago"
            },
            new MeasurementPoint
            {
                TimestampText = latest,
                PointAddress = "AI-0100",
                PointType = "Analog Input",
                GroupVariation = "G30V2",
                ValueText = "229.8 kV",
                Quality = "Online",
                EventClass = "Class 0",
                Context = "Measured feeder voltage",
                Freshness = "Fresh"
            },
            new MeasurementPoint
            {
                TimestampText = latest,
                PointAddress = "BO-0201",
                PointType = "CROB Output",
                GroupVariation = "G12V1",
                ValueText = "READY",
                Quality = "Armed",
                EventClass = "Command",
                Context = "Select / operate candidate",
                Freshness = "Pending live command"
            }
        ];
    }
}
