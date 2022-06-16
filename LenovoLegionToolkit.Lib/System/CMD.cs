using System.Diagnostics;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System
{
    public static class CMD
    {
        public static async Task<string> RunAsync(string file, string arguments)
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
            var output = await cmd.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            await cmd.WaitForExitAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ran [file={file}, argument={arguments}, output={output}]");

            return output;
        }
    }
}
