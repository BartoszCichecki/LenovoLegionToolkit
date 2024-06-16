using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Lib;
using LenovoLegionToolkit.CLI.Lib.Extensions;

namespace LenovoLegionToolkit.CLI;

public static class IpcClient
{
    private static bool PipeExists => Directory.GetFiles(@"\\.\pipe\", Constants.PIPE_NAME).Length > 0;

    public static async Task RunQuickActionAsync(string name)
    {
        if (!PipeExists)
            throw new IpcException("Server unavailable");

        await using var pipe = new NamedPipeClientStream(Constants.PIPE_NAME);

        await ConnectAsync(pipe).ConfigureAwait(false);

        var req = new IpcRequest { Name = name };
        await pipe.WriteObjectAsync(req).ConfigureAwait(false);
        var res = await pipe.ReadObjectAsync<IpcResponse>().ConfigureAwait(false);

        if (res is null || !res.Success)
            throw new IpcException(res?.Message ?? "Unknown failure");
    }

    private static async Task ConnectAsync(NamedPipeClientStream pipe)
    {
        var retries = 3;

        while (retries >= 0)
        {
            try
            {
                await pipe.ConnectAsync(TimeSpan.FromMilliseconds(500), CancellationToken.None).ConfigureAwait(false);
                pipe.ReadMode = PipeTransmissionMode.Message;
                return;
            }
            catch (TimeoutException) { }

            retries--;
        }

        throw new IpcException("Could not connect to server");
    }
}
