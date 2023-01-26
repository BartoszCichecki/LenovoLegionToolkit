using System;
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

    private async void KeyboardBacklightPage_Initialized(object? sender, EventArgs e)
    {
        _titleTextBlock.Visibility = Visibility.Collapsed;

        await Task.WhenAll(
            Task.Delay(TimeSpan.FromMilliseconds(500)),
            _spectrumKeyboardBacklightControl.InitializedTask,
            _rgbKeyboardBacklightControl.InitializedTask,
            _whiteKeyboardBacklightControl.InitializedTask,
            _oneLevelWhiteKeyboardBacklightControl.InitializedTask
        );

        _titleTextBlock.Visibility = Visibility.Visible;

        var spectrum = _spectrumKeyboardBacklightControl.Visibility == Visibility.Visible;
        var rgb = _rgbKeyboardBacklightControl.Visibility == Visibility.Visible;
        var white = _whiteKeyboardBacklightControl.Visibility == Visibility.Visible;
        var oneLevelWhite = _oneLevelWhiteKeyboardBacklightControl.Visibility == Visibility.Visible;
        _noKeyboardsText.Visibility = spectrum || rgb || white || oneLevelWhite ? Visibility.Collapsed : Visibility.Visible;

        _loader.IsLoading = false;
    }
}