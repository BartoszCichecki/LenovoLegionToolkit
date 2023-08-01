using System.Linq;
using System.Windows;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class ExtendedHybridModeInfoWindow
{
    public ExtendedHybridModeInfoWindow(HybridModeState[] hybridModeStates)
    {
        InitializeComponent();

        _hybridPanel.Visibility = hybridModeStates.Contains(HybridModeState.On)
            ? Visibility.Visible
            : Visibility.Collapsed;
        _hybridIgpuPanel.Visibility = hybridModeStates.Contains(HybridModeState.OnIGPUOnly)
            ? Visibility.Visible
            : Visibility.Collapsed;
        _hybridAutoPanel.Visibility = hybridModeStates.Contains(HybridModeState.OnAuto)
            ? Visibility.Visible
            : Visibility.Collapsed;
        _dgpuPanel.Visibility = hybridModeStates.Contains(HybridModeState.Off)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
