using System;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using ProtoBuf;

namespace LenovoLegionToolkit.WPF.Utils;

public class IpcServer(AutomationProcessor automationProcessor, IntegrationsSettings settings)
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private Task? _handler;

    public async Task StartStopIfNeededAsync()
    {
        await StopAsync().ConfigureAwait(false);

        if (!settings.Store.CLI)
            return;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting...");

        _cancellationTokenSource = new();

        var token = _cancellationTokenSource.Token;
        _handler = Task.Run(() => Handler(token), token);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Started");
    }

    public async Task StopAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopping...");

        await _cancellationTokenSource.CancelAsync();

        if (_handler is not null)
            await _handler;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopped");
    }

    private async Task Handler(CancellationToken token)
    {
        try
        {
            var identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            var pipeSecurity = new PipeSecurity();
            pipeSecurity.SetAccessRule(new(identity,
                PipeAccessRights.ReadWrite,
                AccessControlType.Allow));

            await using var pipe = NamedPipeServerStreamAcl.Create(CLI.Lib.Constants.PIPE_NAME,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.None,
                0,
                0,
                pipeSecurity);

            while (!token.IsCancellationRequested)
            {
                await pipe.WaitForConnectionAsync(token).ConfigureAwait(false);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Connection received.");

                try
                {
                    var req = Serializer.DeserializeWithLengthPrefix<IpcRequest>(pipe, PrefixStyle.Base128);
                    await RunQuickActionAsync(req.Name).ConfigureAwait(false);

                    var res = new IpcResponse { Success = true };
                    Serializer.SerializeWithLengthPrefix(pipe, res, PrefixStyle.Base128);
                }
                catch (Exception ex)
                {
                    var res = new IpcResponse { Success = false, Message = ex.Message };
                    Serializer.SerializeWithLengthPrefix(pipe, res, PrefixStyle.Base128);
                }
                finally
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Disconnecting...");

                    pipe.Disconnect();
                }
            }

        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task RunQuickActionAsync(string name)
    {
        var pipelines = await automationProcessor.GetPipelinesAsync();
        var quickAction = pipelines
            .Where(p => p.Trigger is null)
            .FirstOrDefault(p => p.Name == name);

        if (quickAction is null)
            throw new InvalidOperationException($"Quick Action \"{name}\" not found.");

        await automationProcessor.RunNowAsync(quickAction.Id);
    }
}
