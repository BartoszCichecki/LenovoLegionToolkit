using System.Windows.Controls;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum.Device;

public class SpectrumKeyboardControl : UserControl
{
    private readonly SpectrumKeyboardANSIControl _ansi = new();
    private readonly SpectrumKeyboardISOControl _iso = new();

    private readonly StackPanel _stackPanel = new();

    public SpectrumKeyboardControl()
    {
        Content = _stackPanel;
    }

    public void SetLayout(KeyboardLayout keyboardLayout)
    {
        _stackPanel.Children.Remove(_ansi);
        _stackPanel.Children.Remove(_iso);

        switch (keyboardLayout)
        {
            case KeyboardLayout.Ansi:
                _stackPanel.Children.Add(_ansi);
                break;
            case KeyboardLayout.Iso:
                _stackPanel.Children.Add(_iso);
                break;
        }

        UpdateLayout();
    }
}