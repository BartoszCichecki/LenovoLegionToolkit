using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NvAPIWrapper;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.GPU;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal static class NVAPIWrapper
    {
        private static bool _isInitialized = false;

        private static void EnsureInitialized()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            NVIDIA.Initialize();
        }

        public static bool IsGPUPresent(out PhysicalGPU gpu)
        {
            try
            {
                EnsureInitialized();
                gpu = PhysicalGPU.GetPhysicalGPUs().FirstOrDefault(gpu => gpu.SystemType == SystemType.Laptop);
                return gpu != null;
            }
            catch (NVIDIAApiException)
            {
                gpu = null;
                return false;
            }
        }

        public static bool IsGPUActive(PhysicalGPU gpu)
        {
            try
            {
                EnsureInitialized();
                if (gpu == null)
                    return false;
                _ = gpu.PerformanceStatesInfo;
                return true;
            }
            catch (NVIDIAApiException)
            {
                return false;
            }
        }

        public static bool IsDisplayConnected(PhysicalGPU gpu)
        {
            try
            {
                EnsureInitialized();
                if (gpu == null)
                    return false;

                return Display.GetDisplays().Any(d => d.PhysicalGPUs.Contains(gpu, PhysicalGPUEqualityComparer.Instance));
            }
            catch (NVIDIAApiException)
            {
                return false;
            }
        }

        public static string GetGPUId(PhysicalGPU gpu)
        {
            try
            {
                EnsureInitialized();
                if (gpu == null)
                    return null;
                return gpu.BusInformation.PCIIdentifiers.ToString();
            }
            catch (NVIDIAApiException)
            {
                return null;
            }
        }

        private class PhysicalGPUEqualityComparer : IEqualityComparer<PhysicalGPU>
        {
            public static PhysicalGPUEqualityComparer Instance = new();

            private PhysicalGPUEqualityComparer() { }

            public bool Equals(PhysicalGPU x, PhysicalGPU y) => x.GPUId == y.GPUId;

            public int GetHashCode([DisallowNull] PhysicalGPU obj) => obj.GPUId.GetHashCode();
        }
    }
}
