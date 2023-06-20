using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System;

public static class CMD
{
    public static async Task<(int, string)> RunAsync(string file, string arguments, bool waitForExit = true, Dictionary<string, string?>? environment = null)
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

        if (environment is not null)
        {
            foreach (var (key, value) in environment)
                cmd.StartInfo.Environment[key] = value;
        }

        cmd.Start();

        if (!waitForExit)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ran [file={file}, argument={arguments}, waitForExit={waitForExit}, environment=[{(environment is null ? string.Empty : string.Join(",", environment))}]");

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