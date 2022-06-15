using System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class KeyboardBrightnessFeature : AbstractUSBHIDFeature<KeyboardBrightnessState>
    {
        public KeyboardBrightnessFeature() : base(Drivers.GetHid) { }

        protected override LegionRGBKey ToInternal(KeyboardBrightnessState state, LegionRGBKey LegionRGB)
        {
            switch (state)
            {
                case KeyboardBrightnessState.Low:
                    LegionRGB.Brightness = 1;
                    break;
                case KeyboardBrightnessState.High:
                    LegionRGB.Brightness = 2;
                    break;
                default:
                    break;
            }
            return LegionRGB;
        }

        protected override KeyboardBrightnessState FromInternal(LegionRGBKey LegionRGB)
    {
            switch (LegionRGB.Brightness)
            {
               case 1:
                    return KeyboardBrightnessState.Low;
               case 2:
                    return KeyboardBrightnessState.High;
               default :
                    return KeyboardBrightnessState.Low;
            }
           
        }
    }
}
