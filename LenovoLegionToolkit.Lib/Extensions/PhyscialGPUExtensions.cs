using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class NVAPIExtensions
{
    private static readonly string[] _exclusions = new[]
    {
        "dwm.exe",
        "explorer.exe",
    };

    public static List<Process> GetActiveProcesses(PhysicalGPU gpu)
    {
        var processes = new List<Process>();
        var apps = GPUApi.QueryActiveApps(gpu.Handle).Where(app => !_exclusions.Contains(app.ProcessName, StringComparer.InvariantCultureIgnoreCase));

        foreach (var app in apps)
        {
            try
            {
                var process = Process.GetProcessById(app.ProcessId);
                processes.Add(process);
            }
            catch (ArgumentException) { }
        }

        return processes;
    }
}