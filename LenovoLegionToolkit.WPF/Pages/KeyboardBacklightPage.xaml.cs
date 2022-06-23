using System.Windows;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class KeyboardBacklightPage
    {
        public KeyboardBacklightPage()
        {
            InitializeComponent();
        }

        private void KeyboardBacklightControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var rgb = _rgbKeyboardBacklightControl.Visibility == Visibility.Visible;
            var white = _whiteKeyboardBacklightControl.Visibility == Visibility.Visible;
            _noKeyboardsText.Visibility = (rgb || white) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
