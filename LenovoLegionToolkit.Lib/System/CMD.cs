using System.Diagnostics;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System
{
    public static class CMD
    {
        public static async Task<string> RunAsync(string file, string arguments, bool waitForExit = true)
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

            if (!waitForExit)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ran [file={file}, argument={arguments}, waitForExit={waitForExit}]");

                return string.Empty;
            }

            var output = await cmd.StandardOutput.ReadToEndAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ran [file={file}, argument={arguments}, waitForExit={waitForExit}, output={output}]");

            return output;
        }
    }
}
