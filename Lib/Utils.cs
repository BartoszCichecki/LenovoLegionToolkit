using System.Diagnostics;
using System.Management;

namespace LenovoLegionToolkit.Lib
{
    public static class Utils
    {
        public static void RestartWindows()
        {
            var cmd = new Process();
            cmd.StartInfo.FileName = "shutdown";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = $"-r -t 0";
            _ = cmd.Start();
        }

        public static void SetPowerPlan(string guid)
        {
            var cmd = new Process();
            cmd.StartInfo.FileName = "powercfg";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = $"-setactive {guid}";
            _ = cmd.Start();
        }

        public static string GetMachineVersion()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_ComputerSystemProduct");
            foreach (var queryObj in searcher.Get())
                return queryObj["Version"].ToString();
            return null;
        }
    }
}
