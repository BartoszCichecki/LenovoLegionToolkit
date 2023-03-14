using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum.Device;

public partial class SpectrumDeviceKeyboardAndFrontControl
{
    public SpectrumDeviceKeyboardAndFrontControl()
    {
        InitializeComponent();
    }

    public void SetLayout(KeyboardLayout keyboardLayout)
    {
        _keyboard.SetLayout(keyboardLayout);
    }
}
