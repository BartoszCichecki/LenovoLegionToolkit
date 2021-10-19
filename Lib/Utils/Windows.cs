using System.Diagnostics;
using System.Management;

namespace LenovoLegionToolkit.Lib.Utils
{
    public static class Windows
    {
        public static void Restart() => ExecuteProcess("shutdown", "-r -t 0");

        public static void SetPowerPlan(string guid) => ExecuteProcess("powercfg", $"-setactive {guid}");

        public static string GetMachineVersion()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_ComputerSystemProduct");
            foreach (var queryObj in searcher.Get())
                return queryObj["Version"].ToString();
            return null;
        }

        private static bool ExecuteProcess(string file, string arguments)
        {
            var cmd = new Process();
            cmd.StartInfo.FileName = file;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = arguments;
            return cmd.Start();
        }
    }
}
