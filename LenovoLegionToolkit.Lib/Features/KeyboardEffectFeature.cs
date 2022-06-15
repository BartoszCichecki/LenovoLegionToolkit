using System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class KeyboardEffectFeature : AbstractUSBHIDFeature<KeyboardEffectState>
    {
        public KeyboardEffectFeature() : base(Drivers.GetHid) { }

        protected override LegionRGBKey ToInternal(KeyboardEffectState state, LegionRGBKey LegionRGB)
        {
            switch (state)
            {
                case KeyboardEffectState.Static:
                    LegionRGB.Effect = 1;
                    LegionRGB.WAVE_LTR = 0;
                    LegionRGB.WAVE_RTL = 0;
                    break;
                case KeyboardEffectState.Breath:
                    LegionRGB.Effect = 3;
                    LegionRGB.WAVE_LTR = 0;
                    LegionRGB.WAVE_RTL = 0;
                    break;
                case KeyboardEffectState.WaveRTL:
                    LegionRGB.Effect = 4;
                    LegionRGB.WAVE_LTR = 0;
                    LegionRGB.WAVE_RTL = 1;
                    break;
                case KeyboardEffectState.WaveLTR:
                    LegionRGB.Effect = 4;
                    LegionRGB.WAVE_LTR = 1;
                    LegionRGB.WAVE_RTL = 0;
                    break;
                case KeyboardEffectState.Smooth:
                    LegionRGB.Effect = 6;
                    LegionRGB.WAVE_LTR = 0;
                    LegionRGB.WAVE_RTL = 0;
                    break;
                default:
                    break;
            }
            return LegionRGB;
        }

        protected override KeyboardEffectState FromInternal(LegionRGBKey LegionRGB)
        {

            switch (LegionRGB.Effect)
            {
                case 1:
                    return KeyboardEffectState.Static;
                case 2:
                    return KeyboardEffectState.Breath;
                case 4:
                    if ((LegionRGB.WAVE_LTR == 0) & (LegionRGB.WAVE_RTL == 1))
                        return KeyboardEffectState.WaveRTL;
                    else
                        return KeyboardEffectState.WaveLTR;
                case 6:
                    return KeyboardEffectState.Smooth;
                default:
                    return KeyboardEffectState.Static;
            }
        }
    }
}
