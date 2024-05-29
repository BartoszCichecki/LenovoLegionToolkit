using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using ProtoBuf;

namespace LenovoLegionToolkit.Lib.Automation.CmdLine;

public class CmdLineIPCServer
{
    private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();

    private readonly NamedPipeServerStream _pipe = new("LenovoLegionToolkit-IPC-0", PipeDirection.InOut);

    private bool _isRunning;
    private CmdLineQuickActionRunState _state;
    private string? _errmsg;

    public async Task StartAsync()
    {
        _isRunning = true;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting IPC server...");

        await Task.Run(async () =>
        {
            while (_isRunning)
            {
                _pipe.WaitForConnection();

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Connection received.");

                var imsgpack = Serializer.DeserializeWithLengthPrefix<IPCMessagePack>(_pipe, PrefixStyle.Base128);
                if (imsgpack is null)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Deserialize failed.");

                    var omsgpack = new IPCMessagePack()
                    {
                        State = CmdLineQuickActionRunState.DeserializeFailed
                    };
                    Serializer.SerializeWithLengthPrefix(_pipe, omsgpack, PrefixStyle.Base128);
                }
                else
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Running Quick Action \"{imsgpack.QuickActionName}\"...");

                    await RunQuickActionAsync(imsgpack.QuickActionName ?? string.Empty);
                    var omsgpack = new IPCMessagePack()
                    {
                        State = _state,
                        Error = _errmsg
                    };
                    Serializer.SerializeWithLengthPrefix(_pipe, omsgpack, PrefixStyle.Base128);
                }

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Disconnecting...");

                _pipe.Disconnect();
            }
        });
    }

    public Task StopAsync()
    {
        _isRunning = false;
        return Task.CompletedTask;
    }

    private async Task RunQuickActionAsync(string quickActionName)
    {
        var quickAction = _automationProcessor.GetPipelinesAsync().Result
                                                                  .Where(p => p.Trigger is null)
                                                                  .FirstOrDefault(p => p.Name == quickActionName);
        if (quickAction is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Quick Action \"{quickActionName}\" not found.");

            _state = CmdLineQuickActionRunState.ActionNotFound;
            return;
        }

        try
        {
            await quickAction.RunAsync().ConfigureAwait(false);
            _state = CmdLineQuickActionRunState.Ok;
            _errmsg = null;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Run Quick Action failed.", ex);

            _state = CmdLineQuickActionRunState.ActionRunFailed;
            _errmsg = ex.Message;
        }
    }
}
