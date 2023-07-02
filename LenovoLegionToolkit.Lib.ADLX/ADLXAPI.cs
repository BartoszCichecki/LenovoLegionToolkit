namespace LenovoLegionToolkit.Lib.ADLX;

public class ADLXAPI
{
    public static int GetDiscreteGPUUniqueId()
    {
        using var helper = new ADLXHelper();
        helper.Initialize();

        using var services = helper.GetSystemServices();

        var gpuListPtr = global::ADLX.new_gpuListP_Ptr();
        services.GetGPUs(gpuListPtr);
        using var gpuList = global::ADLX.gpuListP_Ptr_value(gpuListPtr);

        for (var it = gpuList.Begin(); it < gpuList.Size(); it++)
        {
            var gpuPtr = global::ADLX.new_gpuP_Ptr();
            gpuList.At(it, gpuPtr);
            using var gpu = global::ADLX.gpuP_Ptr_value(gpuPtr);

            var gpuTypePtr = global::ADLX.new_gpuTypeP();
            gpu.Type(gpuTypePtr);
            var gpuType = global::ADLX.gpuTypeP_value(gpuTypePtr);

            if (gpuType != ADLX_GPU_TYPE.GPUTYPE_DISCRETE)
                continue;

            var uniqueIdPtr = global::ADLX.new_intP();
            gpu.UniqueId(uniqueIdPtr);
            var uniqueId = global::ADLX.intP_value(uniqueIdPtr);
            return uniqueId;
        }

        return -1;
    }
}
