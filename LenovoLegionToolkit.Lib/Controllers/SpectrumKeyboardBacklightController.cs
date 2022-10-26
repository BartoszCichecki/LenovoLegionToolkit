using LenovoLegionToolkit.Lib.System;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class SpectrumKeyboardBacklightController
    {
        private SafeFileHandle? DriverHandle => Devices.GetSpectrumRGBKeyboard();

        public bool IsSupported() => DriverHandle is not null;
    }
}
