using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum;

public class SpectrumKeyboardControl : UserControl
{
    private readonly SpectrumKeyboardANSIControl _ansi = new();

    private readonly SpectrumKeyboardISOControl _iso = new()
    {
        Visibility = Visibility.Collapsed
    };

    public SpectrumKeyboardControl()
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