using DNP3_ProtocolAnalyzer.Core.Abstractions;
using DNP3_ProtocolAnalyzer.Core.Models;
using DNP3_ProtocolAnalyzer.Protocols.Dnp3.Decoding;
using DNP3_ProtocolAnalyzer.Protocols.Dnp3.Mapping;
using DNP3_ProtocolAnalyzer.Protocols.Dnp3.Rules;
using DNP3_ProtocolAnalyzer.Testing.Dnp3;

namespace DNP3_ProtocolAnalyzer.Protocols.Dnp3;

public sealed class Dnp3ProtocolAnalyzer : IProtocolAnalyzer
{
    private readonly IFrameDecoder _frameDecoder = new Dnp3FrameDecoder();
    private readonly Dnp3PointMapBuilder _pointMapBuilder = new();
    private readonly IProtocolRuleSet _ruleSet = new Dnp3BaselineRuleSet();

    public SessionSnapshot Analyze(CaptureSession session)
    {
        var frames = session.Packets.Select(packet => _frameDecoder.Decode(packet)).ToList();
        var points = _pointMapBuilder.Build(frames);
        var issues = _ruleSet.Evaluate(frames, points);
        var events = BuildEvents(session, frames);
        var findings = issues.Select(issue => new FindingItem
        {
            TimestampText = DateTime.Now.ToString("HH:mm:ss"),
            Severity = issue.Severity,
            Category = issue.Title,
            Message = issue.Detail,
            Recommendation = Recommend(issue.Title)
        }).ToList();

        return new SessionSnapshot
        {
            TrafficFrames = frames,
            Points = points,
            Events = events,
            Findings = findings,
            ProtocolContext = BuildProtocolContext(session, frames, points),
            StatusHistory = BuildStatusHistory(session, findings.Count),
            WorkspaceStatus = "Analyzer workspace synchronized",
            AppStatus = "Sample Mode",
            CaptureStatus = "Replay Loaded",
            ProtocolBadge = "Protocol: DNP3 TCP",
            ActiveProfile = $"{session.TransportProfile} | Master 1 <-> Relay 100 | Capture + Analyzer",
            ScenarioSummary = string.Join(", ", Dnp3ScenarioCatalog.Default.Select(s => s.Name))
        };
    }

    private static List<ProtocolEvent> BuildEvents(CaptureSession session, IReadOnlyList<DecodedFrame> frames)
    {
        var events = new List<ProtocolEvent>
        {
            new()
            {
                TimestampText = DateTime.Now.ToString("HH:mm:ss"),
                Source = "Workspace",
                EventType = "Session Ready",
                PointAddress = "-",
                Verdict = "READY",
                EventClass = "System",
                Context = session.TransportProfile,
                Severity = "INFO",
                Category = "Session",
                Message = $"{session.SessionName} analyzed over {session.TransportProfile}."
            }
        };

        foreach (var frame in frames)
        {
            if (frame.FunctionCode == "UNSOLICITED_RESPONSE")
            {
                events.Add(new ProtocolEvent
                {
                    TimestampText = frame.TimestampText,
                    Source = "Relay",
                    EventType = "Unsolicited Event",
                    PointAddress = "BIE-0001",
                    Verdict = "PASS",
                    EventClass = "Class 1",
                    Context = "SOE / spontaneous event path",
                    Severity = "INFO",
                    Category = "Unsolicited",
                    Message = "Relay sent breaker status event without polling."
                });
            }
            else if (frame.FunctionCode == "READ")
            {
                events.Add(new ProtocolEvent
                {
                    TimestampText = frame.TimestampText,
                    Source = "Master",
                    EventType = "Integrity Poll",
                    PointAddress = "Class 0",
                    Verdict = "PENDING",
                    EventClass = "Class 0",
                    Context = "Baseline data refresh",
                    Severity = "INFO",
                    Category = "Polling",
                    Message = "Master requested full Class 0 data refresh."
                });
            }
            else if (frame.FunctionCode == "OPERATE")
            {
                events.Add(new ProtocolEvent
                {
                    TimestampText = frame.TimestampText,
                    Source = "Master",
                    EventType = "Operate CROB",
                    PointAddress = "BO-0201",
                    Verdict = "ACK",
                    EventClass = "Command",
                    Context = "Control lifecycle",
                    Severity = "INFO",
                    Category = "Command",
                    Message = "Control operation issued to point 201."
                });
            }
        }

        return events;
    }

    private static List<ProtocolContextItem> BuildProtocolContext(CaptureSession session, IReadOnlyList<DecodedFrame> frames, IReadOnlyList<MeasurementPoint> points)
    {
        return
        [
            new() { Parameter = "Session", Value = session.SessionName, Updated = DateTime.Now.ToString("HH:mm:ss") },
            new() { Parameter = "Transport", Value = session.TransportProfile, Updated = DateTime.Now.ToString("HH:mm:ss") },
            new() { Parameter = "Endpoints", Value = "Master Address 1 -> Outstation Address 100", Updated = DateTime.Now.ToString("HH:mm:ss") },
            new() { Parameter = "Last Function", Value = frames.LastOrDefault()?.FunctionCode ?? "-", Updated = DateTime.Now.ToString("HH:mm:ss") },
            new() { Parameter = "Last Object", Value = frames.LastOrDefault()?.ObjectSummary ?? "-", Updated = DateTime.Now.ToString("HH:mm:ss") },
            new() { Parameter = "IIN Snapshot", Value = frames.LastOrDefault()?.IinSummary ?? "n/a", Updated = DateTime.Now.ToString("HH:mm:ss") },
            new() { Parameter = "Point Count", Value = points.Count.ToString(), Updated = DateTime.Now.ToString("HH:mm:ss") },
            new() { Parameter = "Scenario Pack", Value = string.Join(", ", Dnp3ScenarioCatalog.Default.Select(x => x.Name)), Updated = DateTime.Now.ToString("HH:mm:ss") }
        ];
    }

    private static List<StatusEntry> BuildStatusHistory(CaptureSession session, int findingCount)
    {
        return
        [
            new() { Severity = "INFO", Code = "BOOT", Message = "DNP3 analyzer workspace bootstrapped.", TimestampText = DateTime.Now.AddSeconds(-20).ToString("yyyy-MM-dd HH:mm:ss") },
            new() { Severity = "INFO", Code = "CAPTURE", Message = $"Capture loaded for {session.TransportProfile}.", TimestampText = DateTime.Now.AddSeconds(-15).ToString("yyyy-MM-dd HH:mm:ss") },
            new() { Severity = "INFO", Code = "PIPELINE", Message = "Frame decode, point projection, and rule evaluation completed.", TimestampText = DateTime.Now.AddSeconds(-10).ToString("yyyy-MM-dd HH:mm:ss") },
            new() { Severity = findingCount == 0 ? "INFO" : "WARN", Code = "FINDINGS", Message = $"Workspace produced {findingCount} findings for operator review.", TimestampText = DateTime.Now.AddSeconds(-5).ToString("yyyy-MM-dd HH:mm:ss") }
        ];
    }

    private static string Recommend(string title)
    {
        return title switch
        {
            "Startup Conditioning" => "Verify relay startup template includes enable unsolicited sequence.",
            "Unsolicited" => "Check unsolicited enable objects and event class assignment on the relay.",
            "Confirm" => "Confirm the master acknowledges unsolicited/event fragments consistently.",
            "Analog Mapping" => "Extend object decoder/projector for analog variations used by SIPROTEC.",
            _ => "Review decoded sequence and compare with expected DNP3 scenario behavior."
        };
    }
}
