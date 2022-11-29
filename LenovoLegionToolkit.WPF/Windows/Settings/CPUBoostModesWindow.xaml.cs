using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.WPF.Controls.Settings;

namespace LenovoLegionToolkit.WPF.Windows.Settings;

public partial class CPUBoostModesWindow
{
    private readonly CPUBoostModeController _cpuBoostController = IoCContainer.Resolve<CPUBoostModeController>();

    public CPUBoostModesWindow() => InitializeComponent();

    private async void CPUBoostModesWindow_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

    private async void CPUBoostModesWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _loader.IsLoading = true;
        _learnMore.Visibility = Visibility.Hidden;

        var loadingTask = Task.Delay(500);

        var settings = await _cpuBoostController.GetSettingsAsync();

        _stackPanel.Children.Clear();
        foreach (var setting in settings)
            _stackPanel.Children.Add(new CPUBoostModeControl(setting));

        await loadingTask;

        _learnMore.Visibility = Visibility.Visible;
        _loader.IsLoading = false;
    }
}