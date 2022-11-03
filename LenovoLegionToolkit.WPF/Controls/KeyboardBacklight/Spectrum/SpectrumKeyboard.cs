using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public class SpectrumKeyboard : UserControl
    {
        private SpectrumKeyboardANSI _ansi = new()
        {
            Visibility = Visibility.Collapsed
        };

        private SpectrumKeyboardISO _iso = new()
        {
            Visibility = Visibility.Collapsed
        };

        public SpectrumKeyboard()
        {
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(_ansi);
            stackPanel.Children.Add(_iso);
            Content = stackPanel;
        }

        public void SetLayout(KeyboardLayout layout)
        {
            switch (layout)
            {
                case KeyboardLayout.Ansi:
                    _ansi.Visibility = Visibility.Visible;
                    _iso.Visibility = Visibility.Collapsed;
                    break;
                case KeyboardLayout.Iso:
                    _ansi.Visibility = Visibility.Collapsed;
                    _iso.Visibility = Visibility.Visible;
                    break;
            }

            UpdateLayout();
        }
    }
}
