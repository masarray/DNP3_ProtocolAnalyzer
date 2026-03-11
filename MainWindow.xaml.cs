using System.Windows;
using DNP3_ProtocolAnalyzer.Core.Services;

namespace DNP3_ProtocolAnalyzer;

public partial class MainWindow : Window
{
    private readonly AnalyzerWorkspaceService _workspaceService;

    public MainWindow()
    {
        InitializeComponent();
        _workspaceService = AnalyzerWorkspaceService.CreateDesignWorkspace();
        DataContext = _workspaceService;
    }

    private void StartSessionButton_Click(object sender, RoutedEventArgs e)
    {
        _workspaceService.StartSession();
    }

    private void LoadSampleButton_Click(object sender, RoutedEventArgs e)
    {
        _workspaceService.LoadSampleCapture();
    }
}
