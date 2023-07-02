using System;
using System.Runtime.InteropServices;

// ReSharper disable IdentifierTypo

namespace LenovoLegionToolkit.Lib.ADL;

public class ADLAPI : IDisposable
{
    public readonly struct Adapter
    {
        public int AdapterIndex { get; }
        public bool IsActive { get; }

        public Adapter(int adapterIndex, bool isActive)
        {
            AdapterIndex = adapterIndex;
            IsActive = isActive;
        }
    }

    private const int VENDOR_ID_AMD = 0x03EA;

    private readonly bool _isInitialized;

    public ADLAPI()
    {
        var result = ADL.ADL_Main_Control_Create?.Invoke(ADL.ADL_Main_Memory_Alloc, 1);
        if (result != ADL.ADL_SUCCESS)
            return;

        _isInitialized = true;
    }

    public Adapter? GetDiscreteAdapter()
    {
        if (!_isInitialized)
            return null;

        var adapterInfoArrayPtr = IntPtr.Zero;

        try
        {
            var numberOfAdapters = 0;
            var result = ADL.ADL_Adapter_NumberOfAdapters_Get?.Invoke(ref numberOfAdapters);
            if (result != ADL.ADL_SUCCESS)
                return null;

            Console.WriteLine($"ADL: numberOfAdapters: {numberOfAdapters}");

            if (numberOfAdapters < 1)
                return null;

            var adapterInfoArray = new ADLAdapterInfoArray();
            var adapterInfoArraySize = Marshal.SizeOf(adapterInfoArray);
            adapterInfoArrayPtr = Marshal.AllocCoTaskMem(adapterInfoArraySize);

            Marshal.StructureToPtr(adapterInfoArray, adapterInfoArrayPtr, false);

            result = ADL.ADL_Adapter_AdapterInfo_Get?.Invoke(adapterInfoArrayPtr, adapterInfoArraySize);
            if (result != ADL.ADL_SUCCESS)
                return null;

            adapterInfoArray = Marshal.PtrToStructure<ADLAdapterInfoArray>(adapterInfoArrayPtr);

            for (var adapterIndex = 0; adapterIndex < numberOfAdapters; adapterIndex++)
            {
                var adapterInfo = adapterInfoArray.ADLAdapterInfo[adapterIndex];

                Console.WriteLine($"ADL: adapterInfo {adapterIndex}: {adapterInfo.VendorID},{adapterInfo.Exist},{adapterInfo.Present}");

                if (adapterInfo.VendorID != VENDOR_ID_AMD)
                    continue;

                if (adapterInfo.Exist == 0)
                    continue;

                if (adapterInfo.Present == 0)
                    continue;

                var asicTypes = 0;
                var valids = 0;
                result = ADL.ADL_Adapter_ASICFamilyType_Get?.Invoke(adapterInfo.AdapterIndex, ref asicTypes, ref valids);
                if (result != ADL.ADL_SUCCESS)
                    return null;

                Console.WriteLine($"ADL: asicTypes {adapterIndex}: {asicTypes},{valids}");

                asicTypes &= valids;
                if ((asicTypes & ADL.ADL_ASIC_DISCRETE) == 0)
                    continue;

                var isActive = 0;
                result = ADL.ADL_Adapter_Active_Get?.Invoke(adapterInfo.AdapterIndex, ref isActive);
                if (result != ADL.ADL_SUCCESS)
                    return null;

                return new(adapterInfo.AdapterIndex, isActive != 0);
            }

            return null;
        }
        finally
        {
            Marshal.FreeCoTaskMem(adapterInfoArrayPtr);
        }
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
        if (!_isInitialized)
            return;

        _ = ADL.ADL_Main_Control_Destroy?.Invoke();
    }
}