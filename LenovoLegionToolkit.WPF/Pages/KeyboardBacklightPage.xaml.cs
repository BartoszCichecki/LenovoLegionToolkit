using System.Threading.Tasks;
using System.Windows;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class KeyboardBacklightPage
    {
        public KeyboardBacklightPage()
        {
            InitializeComponent();

            Loaded += KeyboardBacklightPage_Loaded;
        }

        private async void KeyboardBacklightPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.WhenAll(_rgbKeyboardBacklightControl.FinishedLoadingTask, _whiteKeyboardBacklightControl.FinishedLoadingTask);

            var rgb = _rgbKeyboardBacklightControl.Visibility == Visibility.Visible;
            var white = _whiteKeyboardBacklightControl.Visibility == Visibility.Visible;
            _noKeyboardsText.Visibility = (rgb || white) ? Visibility.Collapsed : Visibility.Visible;

            _loader.IsLoading = false;
        }
    }
}
