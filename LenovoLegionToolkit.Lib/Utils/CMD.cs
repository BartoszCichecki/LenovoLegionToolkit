using System.Diagnostics;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal static class CMD
    {
        public static string Run(string file, string arguments)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Running... [file={file}, argument={arguments}]");

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

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ran [file={file}, argument={arguments}, output={output}]");

            return output;
        }
    }
}
