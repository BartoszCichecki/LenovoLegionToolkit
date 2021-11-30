using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Xml.Linq;

namespace LenovoLegionToolkit.Lib.Utils
{
    public struct MachineInformation
    {
        public string Vendor;
        public string Model;
    }

    public struct NVidiaInformation
    {
        public int ProcessCount;
        public IEnumerable<string> ProcessNames;
    }

    public static class OS
    {
        public static void Restart() => ExecuteProcess("shutdown", "-r -t 0");

        public static void RestartDevice(string _pnpDeviceId) => ExecuteProcess("pnputil", $"/restart-device /deviceid \"{_pnpDeviceId}\"");

        public static void SetPowerPlan(string guid) => ExecuteProcess("powercfg", $"-setactive {guid}");

        public static NVidiaInformation GetNVidiaInformation()
        {
            var output = ExecuteProcessForOutput("nvidia-smi", "-q -x");

            var xdoc = XDocument.Parse(output);
            var gpu = xdoc.Element("nvidia_smi_log").Element("gpu");
            var processInfo = gpu.Element("processes").Elements("process_info");
            var processesCount = processInfo.Count();
            var processNames = processInfo.Select(e => e.Element("process_name").Value).Select(Path.GetFileName);

            return new NVidiaInformation
            {
                ProcessCount = processesCount,
                ProcessNames = processNames,
            };

        }

        public static MachineInformation GetMachineInformation()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_ComputerSystemProduct");
            foreach (var queryObj in searcher.Get())
            {
                var vendor = queryObj["Vendor"].ToString();
                var model = queryObj["Version"].ToString();
                return new MachineInformation
                {
                    Vendor = vendor,
                    Model = model,
                };
            }
            return default;
        }

        private static void ExecuteProcess(string file, string arguments)
        {
            var cmd = new Process();
            cmd.StartInfo.UseShellExecute = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.FileName = file;
            cmd.StartInfo.Arguments = arguments;
            cmd.Start();
            cmd.WaitForExit();
        }

        private static string ExecuteProcessForOutput(string file, string arguments)
        {
            var cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.FileName = file;
            cmd.StartInfo.Arguments = arguments;
            cmd.Start();
            var output = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();
            return output;
        }
    }
}
