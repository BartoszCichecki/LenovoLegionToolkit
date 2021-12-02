using System.Diagnostics;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal static class CMD
    {
        public static void ExecuteProcess(string file, string arguments)
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

        public static string ExecuteProcessForOutput(string file, string arguments)
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
