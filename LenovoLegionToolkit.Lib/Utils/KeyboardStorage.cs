using System;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.Utils
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LegionRGBKey
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Header;
        [MarshalAs(UnmanagedType.U1)]
        public byte Effect;
        [MarshalAs(UnmanagedType.U1)]
        public byte Speed;
        [MarshalAs(UnmanagedType.U1)]
        public byte Brightness;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ZONE1_RGB;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ZONE2_RGB;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ZONE3_RGB;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ZONE4_RGB;
        [MarshalAs(UnmanagedType.U1)]
        public byte Padding;
        [MarshalAs(UnmanagedType.U1)]
        public byte WAVE_LTR;
        [MarshalAs(UnmanagedType.U1)]
        public byte WAVE_RTL;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] Unused;

        public LegionRGBKey()
        {
            Header = new byte[2] { 0xcc, 0x16 };
            Effect = 1;
            Speed = 1;
            Brightness = 1;
            ZONE1_RGB = new byte[3] { 0xFF, 0xFF, 0xFF };
            ZONE2_RGB = new byte[3] { 0xFF, 0xFF, 0xFF };
            ZONE3_RGB = new byte[3] { 0xFF, 0xFF, 0xFF };
            ZONE4_RGB = new byte[3] { 0xFF, 0xFF, 0xFF };
            Padding = 0;
            WAVE_LTR = 0;
            WAVE_RTL = 0;
            Unused = new byte[13];
        }
    }



    //TODO Fix This, might not be Thread Safe
    public sealed class KeyboardData
    {
        private static LegionRGBKey? _LegionRGBKey=null;

        public static LegionRGBKey LegionRGBKey
        {
            get
            {
               
                if (_LegionRGBKey == null)
                {
                        _LegionRGBKey = Settings.Instance.RgbProfile;
                }
                return (LegionRGBKey)_LegionRGBKey;
            }

            set
            {
                _LegionRGBKey = value;
            }
        }
        public static LegionRGBKey BlankLegionRGBKey
        {
            get
            {
                return new LegionRGBKey();
            }
        }
    }
}
