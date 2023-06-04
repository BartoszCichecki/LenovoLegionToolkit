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

        var info = _gpuOverclockController.GetState(PowerModeState.Performance);

        _enableGpuOcCheckbox.IsChecked = info is not null;
        _gpuOcGrid.IsEnabled = info is not null;
        _coreSlider.Minimum = -GPUOverclockController.MAX_CORE_DELTA_MHZ;
        _coreSlider.Maximum = GPUOverclockController.MAX_CORE_DELTA_MHZ;
        _coreSlider.Value = info?.CoreDeltaMhz ?? 0;
        _memorySlider.Minimum = -GPUOverclockController.MAX_MEMORY_DELTA_MHZ;
        _memorySlider.Maximum = GPUOverclockController.MAX_MEMORY_DELTA_MHZ;
        _memorySlider.Value = info?.MemoryDeltaMhz ?? 0;
    }

    private void EnableGpuOcCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        _gpuOcGrid.IsEnabled = _enableGpuOcCheckbox.IsChecked == true;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        GPUOverclockInfo? info = _enableGpuOcCheckbox.IsChecked == true
            ? new GPUOverclockInfo { CoreDeltaMhz = (int)_coreSlider.Value, MemoryDeltaMhz = (int)_memorySlider.Value }
            : null;

        _gpuOverclockController.SaveState(info, PowerModeState.Performance);
        _gpuOverclockController.ApplyState(PowerModeState.Performance);

        await _powerModeFeature.SetStateAsync(PowerModeState.Performance);

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

}
