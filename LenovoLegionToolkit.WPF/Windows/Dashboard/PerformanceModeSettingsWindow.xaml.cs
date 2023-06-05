using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class PerformanceModeSettingsWindow
{
    private readonly GPUOverclockController _gpuOverclockController = IoCContainer.Resolve<GPUOverclockController>();
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();

    public PerformanceModeSettingsWindow()
    {
        InitializeComponent();

        var (enabled, info) = _gpuOverclockController.GetState();

        _enableGpuOcCheckbox.IsChecked = enabled;
        _gpuOcGrid.IsEnabled = enabled;
        _coreSlider.Maximum = GPUOverclockController.MAX_CORE_DELTA_MHZ;
        _coreSlider.Value = info.CoreDeltaMhz;
        _memorySlider.Maximum = GPUOverclockController.MAX_MEMORY_DELTA_MHZ;
        _memorySlider.Value = info.MemoryDeltaMhz;
    }

    private void EnableGpuOcCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        _gpuOcGrid.IsEnabled = _enableGpuOcCheckbox.IsChecked == true;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var enabled = _enableGpuOcCheckbox.IsChecked == true;
        var info = enabled
            ? new GPUOverclockInfo { CoreDeltaMhz = (int)_coreSlider.Value, MemoryDeltaMhz = (int)_memorySlider.Value }
            : GPUOverclockInfo.Zero;

        _gpuOverclockController.SaveState(enabled, info);
        await _gpuOverclockController.ApplyStateAsync();

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

}
