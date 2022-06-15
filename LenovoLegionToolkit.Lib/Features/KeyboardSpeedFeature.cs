using System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class KeyboardSpeedFeature : AbstractUSBHIDFeature<KeyboardSpeedState>
    {
        public KeyboardSpeedFeature() : base(Drivers.GetHid) { }

        protected override LegionRGBKey ToInternal(KeyboardSpeedState state, LegionRGBKey LegionRGB)
        {
            switch (state)
            {
                case KeyboardSpeedState.Slower:
                    LegionRGB.Speed = 1;
                    break;
                case KeyboardSpeedState.Slow:
                    LegionRGB.Speed = 2;
                    break;
                case KeyboardSpeedState.Fast:
                    LegionRGB.Speed = 3;
                    break;
                case KeyboardSpeedState.Faster:
                    LegionRGB.Speed = 4;
                    break;
                default:
                    break;
            }
            return LegionRGB;
        }

        protected override KeyboardSpeedState FromInternal(LegionRGBKey LegionRGB)
        {
            switch (LegionRGB.Speed)
            {
                case 1:
                    return KeyboardSpeedState.Slower;
                case 2:
                    return KeyboardSpeedState.Slow;
                case 3:
                    return KeyboardSpeedState.Fast;
                case 4:
                    return KeyboardSpeedState.Faster;

                default:
                    return KeyboardSpeedState.Slower;
            }
            return KeyboardSpeedState.Slow;
        }
    }
}
