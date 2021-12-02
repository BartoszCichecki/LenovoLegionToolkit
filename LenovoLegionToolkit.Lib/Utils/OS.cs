using System.Diagnostics;
using System.Linq;

namespace LenovoLegionToolkit.Lib.Utils
{
    public static class OS
    {
        public static void Restart() => ExecuteProcess("shutdown", "-r -t 0");

        internal static void RestartDevice(string _pnpDeviceId) => ExecuteProcess("pnputil", $"/restart-device /deviceid \"{_pnpDeviceId}\"");

        internal static NVidiaInformation GetNVidiaInformation()
        {
            var output = ExecuteProcessForOutput("nvidia-smi", "-q -x");
            return NVidiaInformation.Create(output);

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
