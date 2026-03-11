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

    public string WorkspaceStatus
    {
        get => _workspaceStatus;
        private set => SetField(ref _workspaceStatus, value);
    }

    public string AppStatus
    {
        get => _appStatus;
        private set => SetField(ref _appStatus, value);
    }

    public string CaptureStatus
    {
        get => _captureStatus;
        private set => SetField(ref _captureStatus, value);
    }

    public string ProtocolBadge
    {
        get => _protocolBadge;
        private set => SetField(ref _protocolBadge, value);
    }

    public string ActiveProfile
    {
        get => _activeProfile;
        private set => SetField(ref _activeProfile, value);
    }

    public string ScenarioSummary
    {
        get => _scenarioSummary;
        private set => SetField(ref _scenarioSummary, value);
    }

    public string TrafficSummaryText => $"Frames: {TrafficFrames.Count}  Events: {Events.Count}  Findings: {Findings.Count}  Points: {Points.Count}";

    public static AnalyzerWorkspaceService CreateDesignWorkspace()
    {
        var service = new AnalyzerWorkspaceService(new Dnp3ProtocolAnalyzer(), new InMemoryCaptureFeed());
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
        OnPropertyChanged(nameof(TrafficSummaryText));
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
        OnPropertyChanged(nameof(TrafficSummaryText));
        return snapshot;
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
        OnPropertyChanged(nameof(TrafficSummaryText));
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
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
