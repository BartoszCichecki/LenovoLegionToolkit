using System;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Lib;
using LenovoLegionToolkit.CLI.Lib.Extensions;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.CLI;

public class IpcServer(AutomationProcessor automationProcessor, IntegrationsSettings settings)
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private Task _handler = Task.CompletedTask;

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
        await _handler;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopped");
    }

    private async Task Handler(CancellationToken token)
    {
        try
        {
            await using var pipe = new NamedPipeServerStream(LenovoLegionToolkit.CLI.Lib.Constants.PIPE_NAME,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message);

            while (!token.IsCancellationRequested)
            {
                await pipe.WaitForConnectionAsync(token).ConfigureAwait(false);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Connection received.");

                try
                {
                    var req = await pipe.ReadObjectAsync<IpcRequest>(token).ConfigureAwait(false);

                    if (req?.Name is null)
                        throw new IpcException("Failed to deserialize request.");

                    await RunQuickActionAsync(req.Name).ConfigureAwait(false);

                    var res = new IpcResponse { Success = true };
                    await pipe.WriteObjectAsync(res, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var res = new IpcResponse { Success = false, Message = ex.Message };
                    await pipe.WriteObjectAsync(res, token).ConfigureAwait(false);
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
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unknown failure.", ex);
        }
    }

    private async Task RunQuickActionAsync(string name)
    {
        var pipelines = await automationProcessor.GetPipelinesAsync().ConfigureAwait(false);
        var quickAction = pipelines
            .Where(p => p.Trigger is null)
            .FirstOrDefault(p => p.Name == name) ?? throw new InvalidOperationException($"Quick Action \"{name}\" not found.");

        await automationProcessor.RunNowAsync(quickAction.Id);
    }
}
