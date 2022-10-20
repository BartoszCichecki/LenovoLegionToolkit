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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 960)]
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

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_SET_EFFECT
    {
        public LENOVO_SPECTRUM_HEADER Header;
    }
}
