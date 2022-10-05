using System;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FanTable
    {
        public byte FSTM;
        public byte FSID;
        public uint FSTL;
        public ushort FSS0;
        public ushort FSS1;
        public ushort FSS2;
        public ushort FSS3;
        public ushort FSS4;
        public ushort FSS5;
        public ushort FSS6;
        public ushort FSS7;
        public ushort FSS8;
        public ushort FSS9;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public byte[] Padding;

        public FanTable(ushort[] fanTable)
        {
            if (fanTable.Length != 10)
                throw new ArgumentException("Length must be 10.", nameof(fanTable));

            for (var i = 0; i < fanTable.Length; i++)
                fanTable[i] = Math.Clamp(fanTable[i], (ushort)1, (ushort)10u);

            FSTM = 1;
            FSID = 0;
            FSTL = 0;
            FSS0 = fanTable[0];
            FSS1 = fanTable[1];
            FSS2 = fanTable[2];
            FSS3 = fanTable[3];
            FSS4 = fanTable[4];
            FSS5 = fanTable[5];
            FSS6 = fanTable[6];
            FSS7 = fanTable[7];
            FSS8 = fanTable[8];
            FSS9 = fanTable[9];
            Padding = new byte[38];
        }

        public ushort[] GetTable() => new[] { FSS0, FSS1, FSS2, FSS3, FSS4, FSS5, FSS6, FSS7, FSS8, FSS9 };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LenovoBatteryInformationEx
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
    public struct RGBKeyboardStateEx
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
}
