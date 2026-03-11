using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DNP3_ProtocolAnalyzer.Core.Abstractions;
using DNP3_ProtocolAnalyzer.Core.Models;
using DNP3_ProtocolAnalyzer.Infrastructure.Capture;
using DNP3_ProtocolAnalyzer.Protocols.Dnp3;
using DNP3_ProtocolAnalyzer.Testing.Dnp3;

namespace DNP3_ProtocolAnalyzer.Core.Services;

public sealed class AnalyzerWorkspaceService : ISessionService, INotifyPropertyChanged
{
    private readonly IProtocolAnalyzer _protocolAnalyzer;
    private readonly InMemoryCaptureFeed _captureFeed;
    private string _workspaceStatus = "Ready";
    private string _appStatus = "Ready";
    private string _captureStatus = "Capture Idle";
    private string _protocolBadge = "Protocol: DNP3 TCP";
    private string _activeProfile = "TCP Client | DNP3 over TCP | Demo relay workflow";
    private string _scenarioSummary = string.Join(", ", Dnp3ScenarioCatalog.Default.Select(s => s.Name));
    private string _lineMonitorMode = "Semantic";
    private string _measurementMode = "Structured";
    private string _zoomText = "Zoom 100%";

    private AnalyzerWorkspaceService(IProtocolAnalyzer protocolAnalyzer, InMemoryCaptureFeed captureFeed)
    {
        _protocolAnalyzer = protocolAnalyzer;
        _captureFeed = captureFeed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<DecodedFrame> TrafficFrames { get; } = [];
    public ObservableCollection<MeasurementPoint> Points { get; } = [];
    public ObservableCollection<ProtocolEvent> Events { get; } = [];
    public ObservableCollection<FindingItem> Findings { get; } = [];
    public ObservableCollection<ProtocolContextItem> ProtocolContext { get; } = [];
    public ObservableCollection<StatusEntry> StatusHistory { get; } = [];
    public ObservableCollection<ProtocolContextItem> GlossaryItems { get; } = [];

    public string WorkspaceStatus { get => _workspaceStatus; private set => SetField(ref _workspaceStatus, value); }
    public string AppStatus { get => _appStatus; private set => SetField(ref _appStatus, value); }
    public string CaptureStatus { get => _captureStatus; private set => SetField(ref _captureStatus, value); }
    public string ProtocolBadge { get => _protocolBadge; private set => SetField(ref _protocolBadge, value); }
    public string ActiveProfile { get => _activeProfile; private set => SetField(ref _activeProfile, value); }
    public string ScenarioSummary { get => _scenarioSummary; private set => SetField(ref _scenarioSummary, value); }
    public string LineMonitorMode { get => _lineMonitorMode; private set => SetField(ref _lineMonitorMode, value); }
    public string MeasurementMode { get => _measurementMode; private set => SetField(ref _measurementMode, value); }
    public string ZoomText { get => _zoomText; private set => SetField(ref _zoomText, value); }

    public IEnumerable<string> LineMonitorEntries => TrafficFrames.Select(FormatLineMonitorEntry).ToList();
    public string TrafficSummaryText => $"Frames: {TrafficFrames.Count}  Events: {Events.Count}  Findings: {Findings.Count}  Points: {Points.Count}";
    public string StatusSummaryText => $"Info: {StatusHistory.Count(x => x.Severity == "INFO")}  Warn: {StatusHistory.Count(x => x.Severity == "WARN")}  Error: {StatusHistory.Count(x => x.Severity == "ERROR")}";

    public static AnalyzerWorkspaceService CreateDesignWorkspace()
    {
        var service = new AnalyzerWorkspaceService(new Dnp3ProtocolAnalyzer(), new InMemoryCaptureFeed());
        service.LoadGlossary();
        service.LoadSampleCapture();
        return service;
    }

    public CaptureSession StartSession()
    {
        var session = _captureFeed.CreateLiveSession();
        ApplySnapshot(_protocolAnalyzer.Analyze(session));
        InsertStatus("INFO", "SESSION_START", $"Live session initialized at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.");
        AppStatus = "Live Demo";
        CaptureStatus = "Capture Running";
        WorkspaceStatus = "Live sample session initialized";
        RefreshSummaries();
        return session;
    }

    public SessionSnapshot LoadSampleCapture()
    {
        var session = _captureFeed.CreateSampleSession();
        var snapshot = _protocolAnalyzer.Analyze(session);
        ApplySnapshot(snapshot);
        InsertStatus("INFO", "SAMPLE_LOAD", $"Loaded sample scenario pack: {string.Join(", ", Dnp3ScenarioCatalog.Default.Select(s => s.Name))}.");
        AppStatus = string.IsNullOrWhiteSpace(snapshot.AppStatus) ? "Ready" : snapshot.AppStatus;
        CaptureStatus = string.IsNullOrWhiteSpace(snapshot.CaptureStatus) ? "Sample Loaded" : snapshot.CaptureStatus;
        WorkspaceStatus = string.IsNullOrWhiteSpace(snapshot.WorkspaceStatus) ? "Sample DNP3 capture loaded" : snapshot.WorkspaceStatus;
        RefreshSummaries();
        return snapshot;
    }

    public void MarkOperatorAction(string actionName, string detail)
    {
        InsertStatus("INFO", actionName.ToUpperInvariant().Replace(' ', '_'), detail);
        WorkspaceStatus = detail;
        RefreshSummaries();
    }

    public void SetLineMonitorMode(string mode)
    {
        LineMonitorMode = mode;
        MarkOperatorAction("Line Mode", $"Line monitor switched to {mode} mode.");
        OnPropertyChanged(nameof(LineMonitorEntries));
    }

    public void SetMeasurementMode(string mode)
    {
        MeasurementMode = mode;
        MarkOperatorAction("Measurement Mode", $"Measurement view switched to {mode} mode.");
    }

    public void SetZoom(double scale)
    {
        ZoomText = $"Zoom {(int)Math.Round(scale * 100d)}%";
    }

    private void ApplySnapshot(SessionSnapshot snapshot)
    {
        ReplaceCollection(TrafficFrames, snapshot.TrafficFrames);
        ReplaceCollection(Points, snapshot.Points);
        ReplaceCollection(Events, snapshot.Events);
        ReplaceCollection(Findings, snapshot.Findings);
        ReplaceCollection(ProtocolContext, snapshot.ProtocolContext);
        ReplaceCollection(StatusHistory, snapshot.StatusHistory);
        ActiveProfile = string.IsNullOrWhiteSpace(snapshot.ActiveProfile) ? ActiveProfile : snapshot.ActiveProfile;
        ScenarioSummary = string.IsNullOrWhiteSpace(snapshot.ScenarioSummary) ? ScenarioSummary : snapshot.ScenarioSummary;
        ProtocolBadge = string.IsNullOrWhiteSpace(snapshot.ProtocolBadge) ? ProtocolBadge : snapshot.ProtocolBadge;
        AppStatus = string.IsNullOrWhiteSpace(snapshot.AppStatus) ? AppStatus : snapshot.AppStatus;
        CaptureStatus = string.IsNullOrWhiteSpace(snapshot.CaptureStatus) ? CaptureStatus : snapshot.CaptureStatus;
        WorkspaceStatus = string.IsNullOrWhiteSpace(snapshot.WorkspaceStatus) ? WorkspaceStatus : snapshot.WorkspaceStatus;
        RefreshSummaries();
    }

    private void LoadGlossary()
    {
        GlossaryItems.Clear();
        GlossaryItems.Add(new ProtocolContextItem { Parameter = "Function", Value = "READ", Updated = "Integrity / Class polls" });
        GlossaryItems.Add(new ProtocolContextItem { Parameter = "Function", Value = "RESPONSE", Updated = "Solicited response fragment" });
        GlossaryItems.Add(new ProtocolContextItem { Parameter = "Function", Value = "UNSOLICITED_RESPONSE", Updated = "Spontaneous Class 1/2/3 event" });
        GlossaryItems.Add(new ProtocolContextItem { Parameter = "Object", Value = "G1V2", Updated = "Binary Input with flags" });
        GlossaryItems.Add(new ProtocolContextItem { Parameter = "Object", Value = "G2V2", Updated = "Binary Event with flags" });
        GlossaryItems.Add(new ProtocolContextItem { Parameter = "Object", Value = "G12V1", Updated = "Control Relay Output Block" });
        GlossaryItems.Add(new ProtocolContextItem { Parameter = "Object", Value = "G30V2", Updated = "32-bit analog input" });
        GlossaryItems.Add(new ProtocolContextItem { Parameter = "IIN", Value = "DEVICE_RESTART", Updated = "Outstation restart indication" });
    }

    private void InsertStatus(string severity, string code, string message)
    {
        StatusHistory.Insert(0, new StatusEntry
        {
            Severity = severity,
            Code = code,
            Message = message,
            TimestampText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    private void RefreshSummaries()
    {
        OnPropertyChanged(nameof(LineMonitorEntries));
        OnPropertyChanged(nameof(TrafficSummaryText));
        OnPropertyChanged(nameof(StatusSummaryText));
    }

    private string FormatLineMonitorEntry(DecodedFrame frame)
    {
        var directionMark = frame.Direction == "RX" ? "<=" : "=>";
        var prefix = $"[{frame.TimestampText}] {directionMark} {frame.Source} -> {frame.Destination}";

        return LineMonitorMode == "RAW Classic"
            ? $"{prefix}\n    RAW: {frame.RawHex}"
            : $"{prefix}\n    FC: {frame.FunctionCode} | OBJ: {frame.ObjectSummary} | TAG: {frame.SemanticTag}\n    TP: {frame.Transport} | APP: {frame.ApplicationControl} | IIN: {frame.IinSummary}\n    NOTE: {frame.Summary}\n    RAW: {frame.RawHex}";
    }

    private static void ReplaceCollection<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (var item in source)
        {
            target.Add(item);
        }
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
