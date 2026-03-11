using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DNP3_ProtocolAnalyzer.Core.Services;

namespace DNP3_ProtocolAnalyzer;

public partial class MainWindow : Window
{
    private readonly AnalyzerWorkspaceService _workspaceService;
    private double _uiScale = 1.0d;
    private const double MinUiScale = 0.80d;
    private const double MaxUiScale = 1.30d;
    private const double UiScaleStep = 0.10d;

    public MainWindow()
    {
        InitializeComponent();
        _workspaceService = AnalyzerWorkspaceService.CreateDesignWorkspace();
        DataContext = _workspaceService;
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ApplyResponsiveWindowBounds();
        ApplyUiScale();
        ApplyStatusHistoryLayout(StatusHistoryExpander.IsExpanded);
    }

    private void ConnectionSetupButton_Click(object sender, RoutedEventArgs e)
    {
        _workspaceService.MarkOperatorAction("Connection Setup", "Connection setup workflow placeholder opened for DNP3 TCP/serial profile.");
        MessageBox.Show(this, "Connection Setup belum diimplementasikan penuh. Tahap berikutnya: profile TCP/serial, endpoint, outstation address, timeout, dan unsolicited policy.", "DNP3 Connection Setup", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void StartSessionButton_Click(object sender, RoutedEventArgs e)
    {
        _workspaceService.StartSession();
    }

    private void LoadSampleButton_Click(object sender, RoutedEventArgs e)
    {
        _workspaceService.LoadSampleCapture();
    }

    private void SaveCaptureButton_Click(object sender, RoutedEventArgs e)
    {
        _workspaceService.MarkOperatorAction("Save Capture", "Save capture workflow placeholder selected.");
        MessageBox.Show(this, "Save Capture Data akan diarahkan ke format replay/session DNP3 internal pada fase berikutnya.", "Save Capture Data", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OpenCaptureButton_Click(object sender, RoutedEventArgs e)
    {
        _workspaceService.MarkOperatorAction("Open Capture", "Open capture workflow placeholder selected.");
        MessageBox.Show(this, "Open Capture Data akan dipakai untuk replay file, sample session, dan import hasil lapangan.", "Open Capture Data", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ExportDataButton_Click(object sender, RoutedEventArgs e)
    {
        _workspaceService.MarkOperatorAction("Export Data", "Export data workflow placeholder selected.");
        MessageBox.Show(this, "Export Data akan mencakup event log, findings, measurement snapshot, dan summary report.", "Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void LineMonitorModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || sender is not ComboBox combo || combo.SelectedItem is not ComboBoxItem item) return;
        _workspaceService.SetLineMonitorMode(item.Content?.ToString() ?? "Semantic");
    }

    private void MeasurementModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || sender is not ComboBox combo || combo.SelectedItem is not ComboBoxItem item) return;
        _workspaceService.SetMeasurementMode(item.Content?.ToString() ?? "Structured");
    }

    private void StatusHistoryExpander_Expanded(object sender, RoutedEventArgs e) => ApplyStatusHistoryLayout(true);
    private void StatusHistoryExpander_Collapsed(object sender, RoutedEventArgs e) => ApplyStatusHistoryLayout(false);

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control) return;

        if (e.Key is Key.OemPlus or Key.Add)
        {
            _uiScale = Math.Min(MaxUiScale, _uiScale + UiScaleStep);
            ApplyUiScale();
            e.Handled = true;
            return;
        }

        if (e.Key is Key.OemMinus or Key.Subtract)
        {
            _uiScale = Math.Max(MinUiScale, _uiScale - UiScaleStep);
            ApplyUiScale();
            e.Handled = true;
            return;
        }

        if (e.Key is Key.D0 or Key.NumPad0)
        {
            _uiScale = 1.0d;
            ApplyUiScale();
            e.Handled = true;
        }
    }

    private void ApplyUiScale()
    {
        MainLayoutScaleTransform.ScaleX = _uiScale;
        MainLayoutScaleTransform.ScaleY = _uiScale;
        _workspaceService.SetZoom(_uiScale);
    }

    private void ApplyStatusHistoryLayout(bool expanded)
    {
        StatusSplitterRow.Height = expanded ? new GridLength(6) : new GridLength(0);
        StatusHistoryRow.Height = expanded ? new GridLength(155) : new GridLength(32);
        StatusGridSplitter.Visibility = expanded ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ApplyResponsiveWindowBounds()
    {
        Rect workArea = SystemParameters.WorkArea;
        const double margin = 24d;
        double targetWidth = Math.Min(1550d, Math.Max(1120d, workArea.Width - margin));
        double targetHeight = Math.Min(930d, Math.Max(700d, workArea.Height - margin));

        MaxWidth = workArea.Width;
        MaxHeight = workArea.Height;
        Width = targetWidth;
        Height = targetHeight;
        Left = workArea.Left + Math.Max(0d, (workArea.Width - targetWidth) / 2d);
        Top = workArea.Top + Math.Max(0d, (workArea.Height - targetHeight) / 2d);

        if (workArea.Width < 1360d || workArea.Height < 840d)
        {
            WindowState = WindowState.Maximized;
        }
    }
}
