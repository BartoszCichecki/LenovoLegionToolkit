using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.CLI.Datapacks;
using ProtoBuf;

namespace LenovoLegionToolkit.Lib.CLI;

public class CmdLineIPCClient
{
    private readonly NamedPipeClientStream _pipe = new(".", "LenovoLegionToolkit-IPC-0", PipeDirection.InOut, PipeOptions.None);

    public CLIQuickActionRunState QuickActionState { get; private set; }
    public string? Errmsg { get; private set; }

    public async Task RunQuickActionAsync(string quickActionName)
    {
        if (!CheckPipeExists())
        {
            QuickActionState = CLIQuickActionRunState.Status_ServerNotRunning;
            return;
        }

        try
        {
            await ConnectToIPCServerAsync();
        }
        catch (Exception ex)
        {
            QuickActionState = CLIQuickActionRunState.Status_PipeConnectFailed;
            Errmsg = ex.Message;
            return;
        }

        var omsgpack = new QuickActionRequest()
        {
            Name = quickActionName
        };
        Serializer.SerializeWithLengthPrefix(_pipe, omsgpack, PrefixStyle.Base128);

        var imsgpack = Serializer.DeserializeWithLengthPrefix<QuickActionResponse>(_pipe, PrefixStyle.Base128);

        if (imsgpack is null)
        {
            QuickActionState = CLIQuickActionRunState.Undefined;
        }
        else
        {
            QuickActionState = imsgpack.State;
            Errmsg = imsgpack.Error;
        }
    }

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
                if (!CheckPipeExists())
                {
                    throw new OperationCanceledException();
                }
            }
        }
        throw new OperationCanceledException();
    }

    private static bool CheckPipeExists() => Directory.GetFiles(@"\\.\pipe\", "LenovoLegionToolkit-IPC-0").Any();
}
