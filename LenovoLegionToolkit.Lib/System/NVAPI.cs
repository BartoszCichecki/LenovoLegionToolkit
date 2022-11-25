using System;
using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.GPU;

namespace LenovoLegionToolkit.Lib.System
{
    internal static class NVAPI
    {
        public static void Initialize() => NVIDIA.Initialize();

        public static void Unload() => NVIDIA.Unload();

        public static PhysicalGPU? GetGPU()
        {
            try
            {
                return PhysicalGPU.GetPhysicalGPUs().FirstOrDefault(gpu => gpu.SystemType == SystemType.Laptop);
            }
            catch (NVIDIAApiException)
            {
                return null;
            }
        }

        public static bool IsDisplayConnected(PhysicalGPU gpu)
        {
            try
            {
                return Display.GetDisplays().Any(d => d.PhysicalGPUs.Contains(gpu, PhysicalGPUEqualityComparer.Instance));
            }
            catch (NVIDIAApiException)
            {
                return false;
            }
        }

        public static string? GetGPUId(PhysicalGPU gpu)
        {
            try
            {
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

            public bool Equals(PhysicalGPU? x, PhysicalGPU? y) => x?.GPUId == y?.GPUId;

            public int GetHashCode(PhysicalGPU obj) => obj.GPUId.GetHashCode();
        }
    }
}
