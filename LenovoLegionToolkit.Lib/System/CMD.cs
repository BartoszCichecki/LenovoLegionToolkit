using System.Diagnostics;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System;

public static class CMD
{
    public static async Task<(int, string)> RunAsync(string file, string arguments, bool waitForExit = true)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Running... [file={file}, argument={arguments}]");

        var cmd = new Process();
        cmd.StartInfo.UseShellExecute = false;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        cmd.StartInfo.FileName = file;
        if (!string.IsNullOrWhiteSpace(arguments))
            cmd.StartInfo.Arguments = arguments;
        cmd.Start();

        if (!waitForExit)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ran [file={file}, argument={arguments}, waitForExit={waitForExit}]");

            return (-1, string.Empty);
        }

        await cmd.WaitForExitAsync().ConfigureAwait(false);

        var exitCode = cmd.ExitCode;
        var output = await cmd.StandardOutput.ReadToEndAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Ran [file={file}, argument={arguments}, waitForExit={waitForExit}, exitCode={exitCode} output={output}]");

        return (exitCode, output);
    }
}