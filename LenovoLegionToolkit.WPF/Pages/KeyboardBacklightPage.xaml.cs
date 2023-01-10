using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Pages;

public partial class KeyboardBacklightPage
{
    public KeyboardBacklightPage()
    {
        InitializeComponent();

        Loaded += KeyboardBacklightPage_Loaded;
    }

    public static async Task<bool> IsSupportedAsync()
    {
        var spectrumController = IoCContainer.Resolve<SpectrumKeyboardBacklightController>();
        if (await spectrumController.IsSupportedAsync())
            return true;

        var rgbController = IoCContainer.Resolve<RGBKeyboardBacklightController>();
        if (await rgbController.IsSupportedAsync())
            return true;

        var whiteKeyboardBacklightFeature = IoCContainer.Resolve<WhiteKeyboardBacklightFeature>();
        if (await whiteKeyboardBacklightFeature.IsSupportedAsync())
            return true;

        var oneLevelWhiteKeyboardBacklightFeature = IoCContainer.Resolve<OneLevelWhiteKeyboardBacklightFeature>();
        if (await oneLevelWhiteKeyboardBacklightFeature.IsSupportedAsync())
            return true;

        return false;
    }

    private async void KeyboardBacklightPage_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.WhenAll(
            _spectrumKeyboardBacklightControl.FinishedLoadingTask,
            _rgbKeyboardBacklightControl.FinishedLoadingTask,
            _whiteKeyboardBacklightControl.FinishedLoadingTask,
            _oneLevelWhiteKeyboardBacklightControl.FinishedLoadingTask,
            Task.Delay(1000)
        );

        var spectrum = _spectrumKeyboardBacklightControl.Visibility == Visibility.Visible;
        var rgb = _rgbKeyboardBacklightControl.Visibility == Visibility.Visible;
        var white = _whiteKeyboardBacklightControl.Visibility == Visibility.Visible;
        var oneLevelWhite = _oneLevelWhiteKeyboardBacklightControl.Visibility == Visibility.Visible;
        _noKeyboardsText.Visibility = spectrum || rgb || white || oneLevelWhite ? Visibility.Collapsed : Visibility.Visible;

        _loader.IsLoading = false;
    }
}