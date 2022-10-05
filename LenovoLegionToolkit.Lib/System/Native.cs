using System;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.System
{
    internal enum BatteryQueryInformationLevelEx
    {
        BatteryInformation = 0,
        BatteryGranularityInformation = 1,
        BatteryTemperature = 2,
        BatteryEstimatedTime = 3,
        BatteryDeviceName = 4,
        BatteryManufactureDate = 5,
        BatteryManufactureName = 6,
        BatteryUniqueID = 7
    }

    [Flags]
    internal enum PowerStateEx : uint
    {
        BATTERY_POWER_ONLINE = 1,
        BATTERY_DISCHARGING = 2,
        BATTERY_CHARGING = 4,
        BATTERY_CRITICAL = 8
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BatteryInformationEx
    {
        public int Capabilities;
        public byte Technology;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Chemistry;
        public int DesignedCapacity;
        public int FullChargedCapacity;
        public int DefaultAlert1;
        public int DefaultAlert2;
        public int CriticalBias;
        public int CycleCount;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BatteryStatusEx
    {
        public PowerStateEx PowerState;
        public uint Capacity;
        public uint Voltage;
        public int Rate;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BatteryQueryInformationEx
    {
        public uint BatteryTag;
        public BatteryQueryInformationLevelEx InformationLevel;
        public int AtRate;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BatteryWaitStatusEx
    {
        public uint BatteryTag;
        public uint Timeout;
        public PowerStateEx PowerState;
        public uint LowCapacity;
        public uint HighCapacity;
    }

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
