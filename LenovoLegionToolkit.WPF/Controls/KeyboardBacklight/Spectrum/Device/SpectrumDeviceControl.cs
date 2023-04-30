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
    private readonly SpectrumDeviceKeyboardAndFrontControl _keyboardAndFront = new();
    private readonly SpectrumDeviceKeyboardOnlyControl _keyboardOnly = new();

    private readonly StackPanel _stackPanel = new();

    public SpectrumDeviceControl()
    {
        Content = _stackPanel;
    }

    public void SetLayout(SpectrumLayout spectrumLayout, KeyboardLayout keyboardLayout, HashSet<ushort> keys)
    {
        _stackPanel.Children.Remove(_full);
        _stackPanel.Children.Remove(_keyboardAndFront);
        _stackPanel.Children.Remove(_keyboardOnly);

        switch (spectrumLayout)
        {
            case SpectrumLayout.Full:
                _stackPanel.Children.Add(_full);
                _full.SetLayout(keyboardLayout);
                break;
            case SpectrumLayout.KeyboardAndFront:
                _stackPanel.Children.Add(_keyboardAndFront);
                _keyboardAndFront.SetLayout(keyboardLayout);
                break;
            case SpectrumLayout.KeyboardOnly:
                _stackPanel.Children.Add(_keyboardOnly);
                _keyboardOnly.SetLayout(keyboardLayout);
                break;
        }

        UpdateLayout();

        foreach (var button in GetButtons())
            button.Visibility = keys.Contains(button.KeyCode) ? Visibility.Visible : Visibility.Hidden;
    }

    public IEnumerable<SpectrumZoneControl> GetVisibleButtons() =>
        GetButtons().Where(b => b.Visibility == Visibility.Visible);

    private IEnumerable<SpectrumZoneControl> GetButtons() =>
        this.GetVisibleChildrenOfType<SpectrumZoneControl>()
            .Where(b => b.KeyCode > 0);

    public IEnumerable<SpectrumZoneControl> GetVisibleKeyboardButtons() =>
        GetKeyboardButtons().Where(b => b.Visibility == Visibility.Visible);

    private IEnumerable<SpectrumZoneControl> GetKeyboardButtons() =>
        this.GetVisibleChildrenOfType<SpectrumZoneControl>()
            .Where(b => b.KeyCode is > 0 and < 0x100);
}