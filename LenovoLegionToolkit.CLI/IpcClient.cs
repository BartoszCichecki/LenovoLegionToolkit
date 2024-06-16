using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Lib;
using ProtoBuf;

namespace LenovoLegionToolkit.CLI;

public static class IpcClient
{
    private static bool PipeExists => Directory.GetFiles(@"\\.\pipe\", Constants.PIPE_NAME).Length > 0;

    public static async Task RunQuickActionAsync(string name)
    {
        if (!PipeExists)
            throw new IpcException("Pipe already exists");

        await using var pipe = new NamedPipeClientStream(Constants.PIPE_NAME);
        await ConnectAsync(pipe).ConfigureAwait(false);

        var req = new IpcRequest { Name = name };
        Serializer.SerializeWithLengthPrefix(pipe, req, PrefixStyle.Base128);

        var res = Serializer.DeserializeWithLengthPrefix<IpcResponse>(pipe, PrefixStyle.Base128);

        if (!res.Success)
            throw new IpcException(res.Message ?? "Unknown failure");
    }

    private static async Task ConnectAsync(NamedPipeClientStream pipe)
    {
        var retries = 3;

        while (retries >= 0)
        {
            try
            {
                await pipe.ConnectAsync().ConfigureAwait(false);
                return;
            }
            catch (TimeoutException) { }

            retries--;
        }
    }
}
