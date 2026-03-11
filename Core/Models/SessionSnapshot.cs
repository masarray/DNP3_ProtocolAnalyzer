using System.Collections.Generic;

namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class SessionSnapshot
{
    public IReadOnlyList<DecodedFrame> TrafficFrames { get; init; } = new List<DecodedFrame>();
    public IReadOnlyList<MeasurementPoint> Points { get; init; } = new List<MeasurementPoint>();
    public IReadOnlyList<ProtocolEvent> Events { get; init; } = new List<ProtocolEvent>();
    public IReadOnlyList<FindingItem> Findings { get; init; } = new List<FindingItem>();
    public IReadOnlyList<ProtocolContextItem> ProtocolContext { get; init; } = new List<ProtocolContextItem>();
    public IReadOnlyList<StatusEntry> StatusHistory { get; init; } = new List<StatusEntry>();
    public string WorkspaceStatus { get; init; } = string.Empty;
    public string AppStatus { get; init; } = string.Empty;
    public string CaptureStatus { get; init; } = string.Empty;
    public string ProtocolBadge { get; init; } = string.Empty;
    public string ActiveProfile { get; init; } = string.Empty;
    public string ScenarioSummary { get; init; } = string.Empty;
}
