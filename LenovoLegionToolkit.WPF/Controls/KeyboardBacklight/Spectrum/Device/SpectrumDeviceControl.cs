using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum.Device;

public class SpectrumDeviceControl : UserControl
{
    private readonly SpectrumDeviceFullControl _full = new();

    private readonly SpectrumDeviceKeyboardAndFrontControl _keyboardAndFront = new()
    {
        Visibility = Visibility.Collapsed
    };

    private readonly SpectrumDeviceKeyboardOnlyControl _keyboardOnly = new()
    {
        Visibility = Visibility.Collapsed
    };

    public SpectrumDeviceControl()
    {
        var stackPanel = new StackPanel();
        stackPanel.Children.Add(_full);
        stackPanel.Children.Add(_keyboardAndFront);
        stackPanel.Children.Add(_keyboardOnly);
        Content = stackPanel;
    }

    public void SetLayout(SpectrumLayout spectrumLayout, KeyboardLayout keyboardLayout, HashSet<ushort> keys)
    {
        switch (spectrumLayout)
        {
            case SpectrumLayout.Full:
                _full.SetLayout(keyboardLayout);
                _full.Visibility = Visibility.Visible;
                _keyboardAndFront.Visibility = Visibility.Collapsed;
                _keyboardOnly.Visibility = Visibility.Collapsed;
                break;
            case SpectrumLayout.KeyboardAndFront:
                _keyboardAndFront.SetLayout(keyboardLayout);
                _full.Visibility = Visibility.Collapsed;
                _keyboardAndFront.Visibility = Visibility.Visible;
                _keyboardOnly.Visibility = Visibility.Collapsed;
                break;
            case SpectrumLayout.KeyboardOnly:
                _keyboardOnly.SetLayout(keyboardLayout);
                _full.Visibility = Visibility.Collapsed;
                _keyboardAndFront.Visibility = Visibility.Collapsed;
                _keyboardOnly.Visibility = Visibility.Visible;
                break;
        }

        UpdateLayout();

        foreach (var button in GetButtons())
            button.Visibility = keys.Contains(button.KeyCode) ? Visibility.Visible : Visibility.Hidden;
    }

    public IEnumerable<SpectrumZoneControl> GetVisibleButtons() =>
        GetButtons().Where(b => b.Visibility == Visibility.Visible);

    private IEnumerable<SpectrumZoneControl> GetButtons() =>
        this.GetVisibleChildrenOfType<SpectrumZoneControl>().Where(b => b.KeyCode > 0);
}