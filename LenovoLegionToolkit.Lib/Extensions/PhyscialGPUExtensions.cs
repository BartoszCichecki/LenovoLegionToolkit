using System.IO;
using System.Linq;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class NVAPIExtensions
    {
        public static string[] GetActiveProcessNames(PhysicalGPU gpu)
        {
            var apps = GPUApi.QueryActiveApps(gpu.Handle);
            return apps.Select(a => a.ProcessName).Select(name => Path.GetFileName(name)).ToArray();
        }
    }
}
