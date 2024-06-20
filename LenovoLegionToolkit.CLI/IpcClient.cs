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

    public static async Task<string> ListFeatures()
    {
        var req = new IpcRequest
        {
            Operation = IpcRequest.OperationType.ListFeatures,
        };

        return await SendRequestAsync(req).ConfigureAwait(false)
               ?? throw new IpcException("Missing return message.");
    }
    public static async Task<string> ListFeatureValues(string name)
    {
        var req = new IpcRequest
        {
            Operation = IpcRequest.OperationType.ListFeatureValues,
            Name = name,
        };

        return await SendRequestAsync(req).ConfigureAwait(false)
               ?? throw new IpcException("Missing return message.");
    }

    public static Task SetFeature(string name, string value)
    {
        var req = new IpcRequest
        {
            Operation = IpcRequest.OperationType.SetFeature,
            Name = name,
            Value = value
        };

        return SendRequestAsync(req);
    }

    public static async Task<string> GetFeature(string name)
    {
        var req = new IpcRequest
        {
            Operation = IpcRequest.OperationType.GetFeature,
            Name = name
        };

        return await SendRequestAsync(req).ConfigureAwait(false)
               ?? throw new IpcException("Missing return message.");
    }

    public static Task RunQuickActionAsync(string name)
    {
        var req = new IpcRequest
        {
            Operation = IpcRequest.OperationType.QuickAction,
            Name = name
        };

        return SendRequestAsync(req);
    }

    private static async Task<string?> SendRequestAsync(IpcRequest req)
    {
        if (!PipeExists)
            throw new IpcException("Server unavailable");

        await using var pipe = new NamedPipeClientStream(Constants.PIPE_NAME);

        await ConnectAsync(pipe).ConfigureAwait(false);

        await pipe.WriteObjectAsync(req).ConfigureAwait(false);
        var res = await pipe.ReadObjectAsync<IpcResponse>().ConfigureAwait(false);

        if (res is null || !res.Success)
            throw new IpcException(res?.Message ?? "Unknown failure");

        return res.Message;
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
