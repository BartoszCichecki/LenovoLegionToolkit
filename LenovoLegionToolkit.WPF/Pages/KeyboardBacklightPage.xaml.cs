using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Pages
{
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
            if (spectrumController.IsSupported())
                return true;

            var rgbController = IoCContainer.Resolve<RGBKeyboardBacklightController>();
            if (rgbController.IsSupported())
                return true;

            var whiteKeyboardBacklightFeature = IoCContainer.Resolve<WhiteKeyboardBacklightFeature>();
            try
            {
                _ = await whiteKeyboardBacklightFeature.GetStateAsync();
                return true;
            }
            catch { }

            return false;
        }

        private async void KeyboardBacklightPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.WhenAll(
                _spectrumKeyboardBacklightControl.FinishedLoadingTask,
                _rgbKeyboardBacklightControl.FinishedLoadingTask,
                _whiteKeyboardBacklightControl.FinishedLoadingTask,
                Task.Delay(1000)
            );

            var spectrum = _spectrumKeyboardBacklightControl.Visibility == Visibility.Visible;
            var rgb = _rgbKeyboardBacklightControl.Visibility == Visibility.Visible;
            var white = _whiteKeyboardBacklightControl.Visibility == Visibility.Visible;
            _noKeyboardsText.Visibility = spectrum || rgb || white ? Visibility.Collapsed : Visibility.Visible;

            _loader.IsLoading = false;
        }
    }
}
