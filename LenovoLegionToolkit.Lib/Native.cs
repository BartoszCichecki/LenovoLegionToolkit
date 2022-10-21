using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LENOVO_BATTERY_INFORMATION
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] Bytes1;
        public ushort Temperature;
        public ushort ManufactureDate;
        public ushort FirstUseDate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Bytes2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LENOVO_RGB_KEYBOARD_STATE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Header;
        public byte Effect;
        public byte Speed;
        public byte Brightness;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Zone1Rgb;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Zone2Rgb;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Zone3Rgb;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Zone4Rgb;
        public byte Padding;
        public byte WaveLTR;
        public byte WaveRTL;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] Unused;
    }

    internal enum LENOVO_SPECTRUM_OPERATION_TYPE : byte
    {
        ProfileSet1 = 0xC8,
        EffectChange = 0xCB,
        ProfileSet2 = 0xCC,
        Brightness = 0xCE,
        AuroraSendBitmap = 0xA1
    }

    internal enum LENOVO_SPECTRUM_EFFECT_TYPE : byte
    {
        ScreenRainbow = 1,
        RainbowWave = 2,
        ColorChange = 3,
        ColorPulse = 4,
        ColorWave = 5,
        Smooth = 6,
        Rain = 7,
        Ripple = 8,
        AudioBounceLighting = 9,
        AudioRippleLighting = 10,
        Always = 11,
        TypeLighting = 12,
        LegionAuroraSync = 13,
    }

    internal enum LENOVO_SPECTRUM_COLOR_MODE : byte
    {
        None = 0,
        RandomColor = 1,
        ColorList = 2
    }

    // Verify
    internal enum LENOVO_SPECTRUM_SPEED : byte
    {
        Speed1 = 0,
        Speed2 = 1,
        Speed3 = 2
    }

    // Verify
    internal enum LENOVO_SPECTRUM_DIRECTION : byte
    {
        None = 0,
        LTR = 1,
        RTL = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LENOVO_SPECTRUM_COLOR
    {
        public byte Red;
        public byte Green;
        public byte Blue;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LENOVO_SPECTRUM_HEADER
    {
        public byte Head = 7;
        public LENOVO_SPECTRUM_OPERATION_TYPE OperationType;
        public byte PacketSize; // %256
        public byte Tail = 3;

        public LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE operationType, int packetSize) : this()
        {
            OperationType = operationType;
            PacketSize = (byte)(packetSize % 256);
        }
    }

    // Verify
    [StructLayout(LayoutKind.Sequential)]
    internal struct LENOVO_SPECTRUM_EFFECT_HEADER
    {
        byte Head = 0x6;
        byte Param1Header = 0x1;
        LENOVO_SPECTRUM_EFFECT_TYPE EffectType;
        byte Param2Header = 0x2;
        LENOVO_SPECTRUM_SPEED Speed;
        byte Param3Header = 0x3;
        byte Unknown = 0x0;
        byte Param4Header = 0x4;
        LENOVO_SPECTRUM_DIRECTION Direction;
        byte Param5Header = 0x5;
        LENOVO_SPECTRUM_COLOR_MODE ColorMode;
        byte Param6Header = 0x6;
        byte Param7Header = 0x0;

        public LENOVO_SPECTRUM_EFFECT_HEADER(
            LENOVO_SPECTRUM_EFFECT_TYPE effectType,
            LENOVO_SPECTRUM_SPEED speed,
            LENOVO_SPECTRUM_DIRECTION direction,
            LENOVO_SPECTRUM_COLOR_MODE colorMode)
        {
            EffectType = effectType;
            Speed = speed;
            Direction = direction;
            ColorMode = colorMode;
        }
    };

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_SET_BRIGHTHNESS
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Brightness;

        public LENOVO_SPECTRUM_SET_BRIGHTHNESS(byte brightness)
        {
            Header = new(LENOVO_SPECTRUM_OPERATION_TYPE.Brightness, 0xC0);
            Brightness = brightness;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_SET_PROFILE
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Profile;

        public LENOVO_SPECTRUM_SET_PROFILE(byte profile, bool first)
        {
            Header = new(first ? LENOVO_SPECTRUM_OPERATION_TYPE.ProfileSet1 : LENOVO_SPECTRUM_OPERATION_TYPE.ProfileSet2, 0xC0);
            Profile = profile;
        }
    }

    // Verify
    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_SET_EFFECT
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public LENOVO_SPECTRUM_EFFECT_HEADER EffectHeader;
        public byte NumberOfColors;
        public LENOVO_SPECTRUM_COLOR[] Colors;
        public byte NumberOfKeys;
        public ushort[] Keys;
    }
}
