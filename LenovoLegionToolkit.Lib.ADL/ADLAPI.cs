#pragma warning disable

// ReSharper disable all

using System;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.ADL;

public class ADLAPI : IDisposable
{
    public class Adapter
    {
        public int AdapterIndex { get; }
        public string PNPString { get; }
        public bool IsActive { get; }

        public Adapter(int adapterIndex, string pnpString, bool isActive)
        {
            AdapterIndex = adapterIndex;
            PNPString = pnpString;
            IsActive = isActive;
        }
    }

    private const int VENDOR_ID_AMD = 1002;

    private IntPtr _ptr;

    public ADLAPI()
    {
        Marshal.PrelinkAll(typeof(ADL));

        var result = ADL.ADL2_Main_Control_Create(ADL.ADL_Main_Memory_Alloc, 1, out var ptr);
        if (result != ADL.ADL_SUCCESS)
            return;

        _ptr = ptr;
    }

    public Adapter? GetDiscreteAdapter()
    {
        if (_ptr == IntPtr.Zero)
            return null;

        var result = ADL.ADL2_Adapter_NumberOfAdapters_Get(_ptr, out var numberOfAdapters);
        if (result != ADL.ADL_SUCCESS)
            return null;

        if (numberOfAdapters < 1)
            return null;

        result = ADL.ADL2_Adapter_AdapterInfo_Get(_ptr, out var adapterInfoArray, Marshal.SizeOf<ADLAdapterInfoArray>());
        if (result != ADL.ADL_SUCCESS)
            return null;

        for (var adapterIndex = 0; adapterIndex < numberOfAdapters; adapterIndex++)
        {
            var adapterInfo = adapterInfoArray.ADLAdapterInfo[adapterIndex];

            if (adapterInfo.VendorID != VENDOR_ID_AMD)
                continue;

            if (adapterInfo.Exist == 0)
                continue;

            if (adapterInfo.Present == 0)
                continue;

            result = ADL.ADL2_Adapter_ASICFamilyType_Get(_ptr, adapterInfo.AdapterIndex, out var asicTypes, out var valids);
            if (result != ADL.ADL_SUCCESS)
                continue;

            asicTypes &= valids;
            if ((asicTypes & ADL.ADL_ASIC_DISCRETE) == 0)
                continue;

            result = ADL.ADL2_Adapter_Active_Get(_ptr, adapterInfo.AdapterIndex, out var isActive);
            if (result != ADL.ADL_SUCCESS)
                continue;

            return new(adapterInfo.AdapterIndex, adapterInfo.PNPString, isActive == ADL.ADL_TRUE);
        }

        return null;
    }

    public (int supported, int enabled, int version) GetOverdriveInfo(Adapter adapter)
    {
        if (_ptr == IntPtr.Zero)
            return (0, 0, -1);

        var result = ADL.ADL2_Overdrive_Caps(_ptr, adapter.AdapterIndex, out var supported, out var enabled, out var version);
        if (result != ADL.ADL_SUCCESS)
            return (0, 0, -1);

        return (supported, enabled, version);
    }

    ~ADLAPI()
    {
        ReleaseUnmanagedResources();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        if (_ptr == IntPtr.Zero)
            return;

        _ = ADL.ADL2_Main_Control_Destroy(_ptr);
        _ptr = IntPtr.Zero;
    }
}