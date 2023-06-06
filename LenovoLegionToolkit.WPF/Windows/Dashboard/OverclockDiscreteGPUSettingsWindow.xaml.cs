using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class OverclockDiscreteGPUSettingsWindow
{
    private readonly GPUOverclockController _gpuOverclockController = IoCContainer.Resolve<GPUOverclockController>();

    public OverclockDiscreteGPUSettingsWindow()
    {
        InitializeComponent();

        var (enabled, info) = _gpuOverclockController.GetState();

        _applyCloseGrid.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
        _saveGrid.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;

        _coreSlider.Maximum = GPUOverclockController.MAX_CORE_DELTA_MHZ;
        _coreSlider.Value = info.CoreDeltaMhz;
        _memorySlider.Maximum = GPUOverclockController.MAX_MEMORY_DELTA_MHZ;
        _memorySlider.Value = info.MemoryDeltaMhz;
    }

    private void CoreSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => _coreLabel.Content = $"{(int)_coreSlider.Value:+0;-0;0} Mhz";

    private void MemorySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => _memoryLabel.Content = $"{(int)_memorySlider.Value:+0;-0;0} Mhz";

    private async void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        Save();
        await ApplyAsync();
    }

    private async void ApplyAndCloseButton_Click(object sender, RoutedEventArgs e)
    {
        Save();
        await ApplyAsync();
        Close();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        Save();
        Close();
    }

    private void Save()
    {
        var (enabled, _) = _gpuOverclockController.GetState();
        var info = new GPUOverclockInfo
        {
            CoreDeltaMhz = (int)_coreSlider.Value,
            MemoryDeltaMhz = (int)_memorySlider.Value
        };

        _gpuOverclockController.SaveState(enabled, info);
    }

    private async Task ApplyAsync() => await _gpuOverclockController.ApplyStateAsync();
}
