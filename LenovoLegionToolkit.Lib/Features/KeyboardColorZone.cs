using System;
using LenovoLegionToolkit.Lib.Utils;


namespace LenovoLegionToolkit.Lib.Features
{
 public class KeyboardColorZone : AbstractUSBHIDFeature<KeyboardColorState>
    {
        public KeyboardColorZone() : base(Drivers.GetHid) { }

        protected override LegionRGBKey ToInternal(KeyboardColorState state, LegionRGBKey LegionRGB)
        {
            return LegionRGB;
        }

        protected override KeyboardColorState FromInternal(LegionRGBKey LegionRGB)
            {
            return KeyboardColorState.None;
        }
    }
}
