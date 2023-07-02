#pragma warning disable

// ReSharper disable all

using System;
using System.Runtime.InteropServices;
using FARPROC = System.IntPtr;
using HMODULE = System.IntPtr;

namespace LenovoLegionToolkit.Lib.ADL;

internal delegate IntPtr ADL_Main_Memory_Alloc(int size);
internal delegate int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);
internal delegate int ADL_Main_Control_Destroy();
internal delegate int ADL_Adapter_NumberOfAdapters_Get(ref int numAdapters);
internal delegate int ADL_Adapter_AdapterInfo_Get(IntPtr info, int inputSize);
internal delegate int ADL_Adapter_Active_Get(int adapterIndex, ref int status);
internal delegate int ADL_Adapter_ASICFamilyType_Get(int adapterIndex, ref int asicTypes, ref int valids);
internal delegate int ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);

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
    internal const int ADL_MAX_PATH = 256;
    internal const int ADL_MAX_ADAPTERS = 40;
    internal const int ADL_MAX_DISPLAYS = 40;
    internal const int ADL_MAX_DEVICENAME = 32;
    internal const int ADL_PMLOG_MAX_SENSORS = 256;

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

    private static class ADLImport
    {
        internal const string Atiadlxx_FileName = "atiadlxx.dll";
        internal const string Kernel32_FileName = "kernel32.dll";

        [DllImport(Kernel32_FileName)]
        internal static extern HMODULE GetModuleHandle(string moduleName);

        [DllImport(Atiadlxx_FileName)]
        internal static extern int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

        [DllImport(Atiadlxx_FileName)]
        internal static extern int ADL_Main_Control_Destroy();

        [DllImport(Atiadlxx_FileName)]
        internal static extern int ADL_Main_Control_IsFunctionValid(HMODULE module, string procName);

        [DllImport(Atiadlxx_FileName)]
        internal static extern FARPROC ADL_Main_Control_GetProcAddress(HMODULE module, string procName);

        [DllImport(Atiadlxx_FileName)]
        internal static extern int ADL_Adapter_NumberOfAdapters_Get(ref int numAdapters);

        [DllImport(Atiadlxx_FileName)]
        internal static extern int ADL_Adapter_AdapterInfo_Get(IntPtr info, int inputSize);

        [DllImport(Atiadlxx_FileName)]
        internal static extern int ADL_Adapter_Active_Get(int adapterIndex, ref int status);

        [DllImport(Atiadlxx_FileName)]
        internal static extern int ADL_Adapter_ASICFamilyType_Get(int adapterIndex, ref int asicTypes, ref int valids);

        [DllImport(Atiadlxx_FileName)]
        internal static extern int ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);
    }

    private class ADLCheckLibrary
    {
        private HMODULE ADLLibrary = IntPtr.Zero;

        private static ADLCheckLibrary ADLCheckLibrary_ = new ADLCheckLibrary();

        private ADLCheckLibrary()
        {
            try
            {
                if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(IntPtr.Zero, "ADL_Main_Control_Create"))
                    ADLLibrary = ADLImport.GetModuleHandle(ADLImport.Atiadlxx_FileName);
            }
            catch (DllNotFoundException ex)
            {
                Console.WriteLine($"ADL: DllNotFoundException {ex.Message}");
            }
            catch (EntryPointNotFoundException ex)
            {
                Console.WriteLine($"ADL: EntryPointNotFoundException {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ADL: Exception {ex.Message}");
            }
        }

        ~ADLCheckLibrary()
        {
            if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                ADLImport.ADL_Main_Control_Destroy();
        }

        internal static bool IsFunctionValid(string functionName)
        {
            var result = false;

            if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
            {
                if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(ADLCheckLibrary_.ADLLibrary, functionName))
                    result = true;
            }

            return result;
        }

        internal static FARPROC GetProcAddress(string functionName)
        {
            FARPROC result = System.IntPtr.Zero;
            if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                result = ADLImport.ADL_Main_Control_GetProcAddress(ADLCheckLibrary_.ADLLibrary, functionName);

            return result;
        }
    }

    #region ADL_Main_Memory_Alloc

    internal static ADL_Main_Memory_Alloc ADL_Main_Memory_Alloc = ADL_Main_Memory_Alloc_;

    private static IntPtr ADL_Main_Memory_Alloc_(int size)
    {
        var result = Marshal.AllocCoTaskMem(size);
        return result;
    }

    #endregion ADL_Main_Memory_Alloc

    #region ADL_Main_Memory_Free

    internal static void ADL_Main_Memory_Free(IntPtr buffer)
    {
        if (IntPtr.Zero != buffer)
            Marshal.FreeCoTaskMem(buffer);
    }

    #endregion ADL_Main_Memory_Free

    #region ADL_Main_Control_Create

    internal static ADL_Main_Control_Create? ADL_Main_Control_Create
    {
        get
        {
            if (!ADL_Main_Control_Create_Check && null == ADL_Main_Control_Create_)
            {
                ADL_Main_Control_Create_Check = true;
                if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Create"))
                    ADL_Main_Control_Create_ = ADLImport.ADL_Main_Control_Create;
            }

            return ADL_Main_Control_Create_;
        }
    }

    private static ADL_Main_Control_Create? ADL_Main_Control_Create_ = null;
    private static bool ADL_Main_Control_Create_Check = false;

    #endregion ADL_Main_Control_Create

    #region ADL_Main_Control_Destroy

    internal static ADL_Main_Control_Destroy? ADL_Main_Control_Destroy
    {
        get
        {
            if (!ADL_Main_Control_Destroy_Check && null == ADL_Main_Control_Destroy_)
            {
                ADL_Main_Control_Destroy_Check = true;
                if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Destroy"))
                    ADL_Main_Control_Destroy_ = ADLImport.ADL_Main_Control_Destroy;
            }

            return ADL_Main_Control_Destroy_;
        }
    }

    private static ADL_Main_Control_Destroy? ADL_Main_Control_Destroy_ = null;
    private static bool ADL_Main_Control_Destroy_Check = false;

    #endregion ADL_Main_Control_Destroy

    #region ADL_Adapter_NumberOfAdapters_Get

    internal static ADL_Adapter_NumberOfAdapters_Get? ADL_Adapter_NumberOfAdapters_Get
    {
        get
        {
            if (!ADL_Adapter_NumberOfAdapters_Get_Check && null == ADL_Adapter_NumberOfAdapters_Get_)
            {
                ADL_Adapter_NumberOfAdapters_Get_Check = true;
                if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_NumberOfAdapters_Get"))
                    ADL_Adapter_NumberOfAdapters_Get_ = ADLImport.ADL_Adapter_NumberOfAdapters_Get;
            }

            return ADL_Adapter_NumberOfAdapters_Get_;
        }
    }

    private static ADL_Adapter_NumberOfAdapters_Get? ADL_Adapter_NumberOfAdapters_Get_ = null;
    private static bool ADL_Adapter_NumberOfAdapters_Get_Check = false;

    #endregion ADL_Adapter_NumberOfAdapters_Get

    #region ADL_Adapter_AdapterInfo_Get

    internal static ADL_Adapter_AdapterInfo_Get? ADL_Adapter_AdapterInfo_Get
    {
        get
        {
            if (!ADL_Adapter_AdapterInfo_Get_Check && null == ADL_Adapter_AdapterInfo_Get_)
            {
                ADL_Adapter_AdapterInfo_Get_Check = true;
                if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_AdapterInfo_Get"))
                    ADL_Adapter_AdapterInfo_Get_ = ADLImport.ADL_Adapter_AdapterInfo_Get;
            }

            return ADL_Adapter_AdapterInfo_Get_;
        }
    }

    private static ADL_Adapter_AdapterInfo_Get? ADL_Adapter_AdapterInfo_Get_ = null;
    private static bool ADL_Adapter_AdapterInfo_Get_Check = false;

    #endregion ADL_Adapter_AdapterInfo_Get

    #region ADL_Adapter_ASICFamilyType_Get

    internal static ADL_Adapter_ASICFamilyType_Get? ADL_Adapter_ASICFamilyType_Get
    {
        get
        {
            if (!ADL_Adapter_ASICFamilyType_Get_Check && null == ADL_Adapter_ASICFamilyType_Get_)
            {
                ADL_Adapter_ASICFamilyType_Get_Check = true;
                if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_ASICFamilyType_Get"))
                    ADL_Adapter_ASICFamilyType_Get_ = ADLImport.ADL_Adapter_ASICFamilyType_Get;
            }

            return ADL_Adapter_ASICFamilyType_Get_;
        }
    }

    private static ADL_Adapter_ASICFamilyType_Get? ADL_Adapter_ASICFamilyType_Get_ = null;
    private static bool ADL_Adapter_ASICFamilyType_Get_Check = false;

    #endregion ADL_Adapter_Active_Get

    #region ADL_Adapter_Active_Get

    internal static ADL_Adapter_Active_Get? ADL_Adapter_Active_Get
    {
        get
        {
            if (!ADL_Adapter_Active_Get_Check && null == ADL_Adapter_Active_Get_)
            {
                ADL_Adapter_Active_Get_Check = true;
                if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_Active_Get"))
                    ADL_Adapter_Active_Get_ = ADLImport.ADL_Adapter_Active_Get;
            }

            return ADL_Adapter_Active_Get_;
        }
    }

    private static ADL_Adapter_Active_Get? ADL_Adapter_Active_Get_ = null;
    private static bool ADL_Adapter_Active_Get_Check = false;

    #endregion ADL_Adapter_Active_Get

    #region ADL_Display_DisplayInfo_Get

    internal static ADL_Display_DisplayInfo_Get? ADL_Display_DisplayInfo_Get
    {
        get
        {
            if (!ADL_Display_DisplayInfo_Get_Check && null == ADL_Display_DisplayInfo_Get_)
            {
                ADL_Display_DisplayInfo_Get_Check = true;
                if (ADLCheckLibrary.IsFunctionValid("ADL_Display_DisplayInfo_Get"))
                    ADL_Display_DisplayInfo_Get_ = ADLImport.ADL_Display_DisplayInfo_Get;
            }

            return ADL_Display_DisplayInfo_Get_;
        }
    }

    private static ADL_Display_DisplayInfo_Get? ADL_Display_DisplayInfo_Get_ = null;
    private static bool ADL_Display_DisplayInfo_Get_Check = false;

    #endregion ADL_Display_DisplayInfo_Get

}
