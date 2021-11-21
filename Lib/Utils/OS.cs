using System.Diagnostics;
using System.Management;

namespace LenovoLegionToolkit.Lib.Utils
{
    public struct MachineInformation
    {
        public string Vendor;
        public string Model;
    }

    public static class OS
    {
        public static void Restart() => ExecuteProcess("shutdown", "-r -t 0");

        public static void SetPowerPlan(string guid) => ExecuteProcess("powercfg", $"-setactive {guid}");

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
            cmd.StartInfo.FileName = file;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = arguments;
            cmd.Start();
            cmd.WaitForExit();
        }
    }
}
