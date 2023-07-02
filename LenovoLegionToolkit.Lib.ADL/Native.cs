#pragma warning disable

// ReSharper disable all

using System;
using System.Runtime.InteropServices;
using FARPROC = System.IntPtr;
using HMODULE = System.IntPtr;

namespace LenovoLegionToolkit.Lib.ADL;

[StructLayout(LayoutKind.Sequential)]
internal struct ADLAdapterInfo
{
    int Size;
    internal int AdapterIndex;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
    internal string UDID;
    internal int BusNumber;
    internal int DriverNumber;
    internal int FunctionNumber;
    internal int VendorID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
    internal string AdapterName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
    internal string DisplayName;
    internal int Present;
    internal int Exist;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
    internal string DriverPath;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
    internal string DriverPathExt;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
    internal string PNPString;
    internal int OSDisplayIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ADLAdapterInfoArray
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ADL.ADL_MAX_ADAPTERS)]
    internal ADLAdapterInfo[] ADLAdapterInfo;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ADLDisplayID
{
    internal int DisplayLogicalIndex;
    internal int DisplayPhysicalIndex;
    internal int DisplayLogicalAdapterIndex;
    internal int DisplayPhysicalAdapterIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ADLDisplayInfo
{
    internal ADLDisplayID DisplayID;
    internal int DisplayControllerIndex;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
    internal string DisplayName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
    internal string DisplayManufacturerName;
    internal int DisplayType;
    internal int DisplayOutputType;
    internal int DisplayConnector;
    internal int DisplayInfoMask;
    internal int DisplayInfoValue;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ADLSingleSensorData
{
    internal int Supported;
    internal int Value;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ADLPMLogDataOutput
{
    internal int Size;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADL.ADL_PMLOG_MAX_SENSORS)]
    public ADLSingleSensorData[] Sensors;
}

internal static class ADL
{
    internal delegate IntPtr ADL_Main_Memory_Alloc_Delegate(int size);

    internal const string Atiadlxx_FileName = "atiadlxx.dll";
    internal const string Kernel32_FileName = "kernel32.dll";

    internal const int ADL_MAX_PATH = 256;
    internal const int ADL_MAX_ADAPTERS = 40;
    internal const int ADL_MAX_DISPLAYS = 40;
    internal const int ADL_MAX_DEVICENAME = 32;
    internal const int ADL_PMLOG_MAX_SENSORS = 256;

    internal const int ADL_TRUE = 1;
    internal const int ADL_FALSE = 0;

    internal const int ADL_SUCCESS = 0;
    internal const int ADL_FAIL = -1;
    internal const int ADL_DRIVER_OK = 0;

    internal const int ADL_MAX_GLSYNC_PORTS = 8;
    internal const int ADL_MAX_GLSYNC_PORT_LEDS = 8;
    internal const int ADL_MAX_NUM_DISPLAYMODES = 1024;

    internal const int ADL_ASIC_DISCRETE = (1 << 0);
    internal const int ADL_ASIC_EMBEDDED = (1 << 7);
    internal const int ADL_ASIC_FIREGL = ADL_ASIC_WORKSTATION;
    internal const int ADL_ASIC_FIREMV = (1 << 3);
    internal const int ADL_ASIC_FIRESTREAM = (1 << 6);
    internal const int ADL_ASIC_FUSION = (1 << 5);
    internal const int ADL_ASIC_INTEGRATED = (1 << 1);
    internal const int ADL_ASIC_UNDEFINED = 0;
    internal const int ADL_ASIC_WORKSTATION = (1 << 2);
    internal const int ADL_ASIC_XGP = (1 << 4);

    internal const int ADL_DISPLAY_CONTYPE_UNKNOWN = 0;
    internal const int ADL_DISPLAY_CONTYPE_VGA = 1;
    internal const int ADL_DISPLAY_CONTYPE_DVI_D = 2;
    internal const int ADL_DISPLAY_CONTYPE_DVI_I = 3;
    internal const int ADL_DISPLAY_CONTYPE_ATICVDONGLE_NTSC = 4;
    internal const int ADL_DISPLAY_CONTYPE_ATICVDONGLE_JPN = 5;
    internal const int ADL_DISPLAY_CONTYPE_ATICVDONGLE_NONI2C_JPN = 6;
    internal const int ADL_DISPLAY_CONTYPE_ATICVDONGLE_NONI2C_NTSC = 7;
    internal const int ADL_DISPLAY_CONTYPE_PROPRIETARY = 8;
    internal const int ADL_DISPLAY_CONTYPE_HDMI_TYPE_A = 10;
    internal const int ADL_DISPLAY_CONTYPE_HDMI_TYPE_B = 11;
    internal const int ADL_DISPLAY_CONTYPE_SVIDEO = 12;
    internal const int ADL_DISPLAY_CONTYPE_COMPOSITE = 13;
    internal const int ADL_DISPLAY_CONTYPE_RCA_3COMPONENT = 14;
    internal const int ADL_DISPLAY_CONTYPE_DISPLAYPORT = 15;
    internal const int ADL_DISPLAY_CONTYPE_EDP = 16;
    internal const int ADL_DISPLAY_CONTYPE_WIRELESSDISPLAY = 17;
    internal const int ADL_DISPLAY_CONTYPE_USB_TYPE_C = 18;

    internal const int ADL_ODN_TEMPERATURE_TYPE_CORE = 1;
    internal const int ADL_ODN_TEMPERATURE_TYPE_MEMORY = 2;
    internal const int ADL_ODN_TEMPERATURE_TYPE_VRM_CORE = 3;
    internal const int ADL_ODN_TEMPERATURE_TYPE_VRM_MEMORY = 4;
    internal const int ADL_ODN_TEMPERATURE_TYPE_LIQUID = 5;
    internal const int ADL_ODN_TEMPERATURE_TYPE_PLX = 6;
    internal const int ADL_ODN_TEMPERATURE_TYPE_HOTSPOT = 7;

    internal static ADL_Main_Memory_Alloc_Delegate ADL_Main_Memory_Alloc = ADL_Main_Memory_Alloc_;

    private static IntPtr ADL_Main_Memory_Alloc_(int size)
    {
        var result = Marshal.AllocCoTaskMem(size);
        return result;
    }

    [DllImport(Atiadlxx_FileName)]
    internal static extern int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc_Delegate callback, int enumConnectedAdapters, out IntPtr ptr);

    [DllImport(Atiadlxx_FileName)]
    internal static extern int ADL2_Main_Control_Destroy(IntPtr ptr);

    [DllImport(Atiadlxx_FileName)]
    internal static extern int ADL2_Adapter_NumberOfAdapters_Get(IntPtr ptr, out int numAdapters);

    [DllImport(Atiadlxx_FileName)]
    internal static extern int ADL2_Adapter_AdapterInfo_Get(IntPtr ptr, out ADLAdapterInfoArray info, int inputSize);

    [DllImport(Atiadlxx_FileName)]
    internal static extern int ADL2_Adapter_Active_Get(IntPtr ptr, int adapterIndex, out int status);

    [DllImport(Atiadlxx_FileName)]
    internal static extern int ADL2_Adapter_ASICFamilyType_Get(IntPtr ptr, int adapterIndex, out int asicTypes, out int valids);

    [DllImport(Atiadlxx_FileName)]
    internal static extern int ADL2_Overdrive_Caps(IntPtr ptr, int adapterIndex, out int supported, out int enabled, out int version);
}
