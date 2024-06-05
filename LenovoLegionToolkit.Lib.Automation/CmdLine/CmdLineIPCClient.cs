using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using ProtoBuf;

namespace LenovoLegionToolkit.Lib.Automation.CmdLine;

public class CmdLineIPCClient
{
    private readonly NamedPipeClientStream _pipe = new(".", "LenovoLegionToolkit-IPC-0", PipeDirection.InOut, PipeOptions.None);

    public CmdLineQuickActionRunState State { get; private set; }
    public string? Errmsg { get; private set; }

    public async Task RunQuickActionAsync(string quickActionName)
    {
        if (!CheckIsIPCServerStarted())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"IPC server hasn't been started.");

            State = CmdLineQuickActionRunState.ServerNotRunning;
            return;
        }

        try
        {
            await ConnectToIPCServerAsync();
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Pipe connect failed.", ex);

            State = CmdLineQuickActionRunState.PipeConnectFailed;
            Errmsg = ex.Message;
            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Pipe connected, sending Quick Action call: \"{quickActionName}\"...");

        var omsgpack = new IPCMessagePack()
        {
            QuickActionName = quickActionName
        };
        Serializer.SerializeWithLengthPrefix(_pipe, omsgpack, PrefixStyle.Base128);

        var imsgpack = Serializer.DeserializeWithLengthPrefix<IPCMessagePack>(_pipe, PrefixStyle.Base128);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Response received.");

        if (imsgpack is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Deserialize failed.");

            State = CmdLineQuickActionRunState.Undefined;
        }
        else
        {
            State = imsgpack.State;
            Errmsg = imsgpack.Error;

            if (imsgpack.State == CmdLineQuickActionRunState.ActionRunFailed)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Run Quick Action failed due to following reason: {imsgpack.Error ?? string.Empty}");
            }
            else if (imsgpack.State == CmdLineQuickActionRunState.ActionNotFound)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Quick Action not found.");
            }
            else if (imsgpack.State == CmdLineQuickActionRunState.DeserializeFailed)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Server failed to deserialize request.");
            }
            else if (imsgpack.State == CmdLineQuickActionRunState.Ok)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Run Quick Action successfully.");
            }
            else
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Undefined response received");
            }
        }
    }

    private bool CheckIsIPCServerStarted() => Directory.GetFiles(@"\\.\pipe\", "LenovoLegionToolkit-IPC-0").Length == 1;

    private async Task ConnectToIPCServerAsync()
    {
        while (true)
        {
            try
            {
                await _pipe.ConnectAsync(500);
                return;
            }
            catch (TimeoutException)
            {
                if (!CheckIsIPCServerStarted())
                {
                    throw new OperationCanceledException();
                }
            }
        }
        throw new OperationCanceledException();
    }
}
