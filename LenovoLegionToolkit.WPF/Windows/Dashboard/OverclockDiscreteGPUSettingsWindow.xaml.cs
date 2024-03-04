using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class OverclockDiscreteGPUSettingsWindow
{
    private const string MHZ = "MHz";

    private readonly GPUOverclockController _gpuOverclockController = IoCContainer.Resolve<GPUOverclockController>();

    public OverclockDiscreteGPUSettingsWindow()
    {
        InitializeComponent();

        var (enabled, info) = _gpuOverclockController.GetState();

        _applyCloseGrid.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
        _saveGrid.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;

        _coreSlider.Maximum = GPUOverclockController.GetMaxCoreDeltaMhz();
        _coreSlider.Value = info.CoreDeltaMhz;
        _memorySlider.Maximum = GPUOverclockController.GetMaxMemoryDeltaMhz();
        _memorySlider.Value = info.MemoryDeltaMhz;

        _coreLabel.Content = $"{(int)_coreSlider.Value:+0;-0;0} {MHZ}";
        _memoryLabel.Content = $"{(int)_memorySlider.Value:+0;-0;0} {MHZ}";
    }

    private void CoreSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => _coreLabel.Content = $"{(int)_coreSlider.Value:+0;-0;0} {MHZ}";

    private void MemorySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => _memoryLabel.Content = $"{(int)_memorySlider.Value:+0;-0;0} {MHZ}";

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
        var info = new GPUOverclockInfo((int)_coreSlider.Value, (int)_memorySlider.Value);

        _gpuOverclockController.SaveState(enabled, info);
    }

    private async Task ApplyAsync() => await _gpuOverclockController.ApplyStateAsync();
}
