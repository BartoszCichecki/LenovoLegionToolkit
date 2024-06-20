using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Lib;
using LenovoLegionToolkit.CLI.Lib.Extensions;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.CLI.Features;

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

                    if (req?.Operation is null)
                        throw new IpcException("Failed to deserialize request.");

                    var res = await HandleRequest(req).ConfigureAwait(false);
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

    private async Task<IpcResponse> HandleRequest(IpcRequest req)
    {
        string? message;

        switch (req.Operation)
        {
            case IpcRequest.OperationType.QuickAction when req is { Name: not null }:
                await RunQuickActionAsync(req.Name).ConfigureAwait(false);
                return new IpcResponse { Success = true };
            case IpcRequest.OperationType.ListFeatures:
                message = await ListFeaturesAsync();
                return new IpcResponse { Success = true, Message = message };
            case IpcRequest.OperationType.ListFeatureValues when req is { Name: not null }:
                message = await ListFeatureValuesAsync(req.Name);
                return new IpcResponse { Success = true, Message = message };
            case IpcRequest.OperationType.SetFeature when req is { Name: not null, Value: not null }:
                await SetFeatureAsync(req.Name, req.Value).ConfigureAwait(false);
                return new IpcResponse { Success = true };
            case IpcRequest.OperationType.GetFeature when req is { Name: not null }:
                message = await GetFeatureAsync(req.Name);
                return new IpcResponse { Success = true, Message = message };
            default:
                throw new IpcException("Invalid request");
        }
    }

    private async Task RunQuickActionAsync(string name)
    {
        var pipelines = await automationProcessor.GetPipelinesAsync().ConfigureAwait(false);
        var quickAction = pipelines
            .Where(p => p.Trigger is null)
            .FirstOrDefault(p => p.Name == name)
                          ?? throw new InvalidOperationException($"Quick Action \"{name}\" not found.");

        await automationProcessor.RunNowAsync(quickAction.Id);
    }

    private static async Task<string?> ListFeaturesAsync()
    {
        var features = new List<string>();

        foreach (var feature in FeatureRegistry.All)
        {
            if (await feature.IsSupportedAsync().ConfigureAwait(false))
                features.Add(feature.Name);
        }

        return string.Join('\n', features);
    }
    private static async Task<string?> ListFeatureValuesAsync(string name)
    {
        var feature = FeatureRegistry.All.FirstOrDefault(f => f.Name == name)
                      ?? throw new IpcException("Invalid feature.");
        var values = await feature.GetValuesAsync().ConfigureAwait(false);
        return string.Join('\n', values);
    }

    private static async Task SetFeatureAsync(string name, string value)
    {
        var feature = FeatureRegistry.All.FirstOrDefault(f => f.Name == name)
                      ?? throw new IpcException("Invalid feature.");
        await feature.SetValueAsync(value).ConfigureAwait(false);
    }

    private static async Task<string> GetFeatureAsync(string name)
    {
        var feature = FeatureRegistry.All.FirstOrDefault(f => f.Name == name)
                      ?? throw new IpcException("Invalid feature.");
        return await feature.GetValueAsync().ConfigureAwait(false);
    }
}
