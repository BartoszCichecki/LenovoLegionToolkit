using System;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers.Sensors;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Integrations;

public class HWiNFOIntegration
{
    private const string CUSTOM_SENSOR_HIVE = "HKEY_CURRENT_USER";
    private const string CUSTOM_SENSOR_PATH = @"Software\HWiNFO64\Sensors\Custom";
    private const string CUSTOM_SENSOR_GROUP_NAME = "Lenovo Legion Toolkit";
    private const string CPU_SENSOR_NAME = "CPU Fan Speed";
    private const string GPU_SENSOR_NAME = "GPU Fan Speed";

    private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(1);

    private readonly SensorsController _sensorController;
    private readonly IntegrationsSettings _settings;

    private CancellationTokenSource? _cts;
    private Task? _refreshTask;

    public HWiNFOIntegration(SensorsController sensorController, IntegrationsSettings settings)
    {
        _sensorController = sensorController ?? throw new ArgumentNullException(nameof(sensorController));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task StartStopIfNeededAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopping...");

        _cts?.Cancel();

        if (_refreshTask is not null)
            await _refreshTask.ConfigureAwait(false);

        ClearValues();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopped.");

        if (!_settings.Store.HWiNFO)
            return;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting...");

        _cts = new();
        _refreshTask = RefreshLoopAsync(_cts.Token);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Started.");
    }

    private async Task RefreshLoopAsync(CancellationToken token)
    {
        try
        {
            while (true)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    await SetSensorValuesAsync().ConfigureAwait(false);
                    await Task.Delay(_refreshInterval, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Failed to set values.", ex);
                }
            }
        }
        catch (OperationCanceledException) { }
    }

    private async Task SetSensorValuesAsync()
    {
        var (cpuFanSpeed, gpuFanSpeed) = await _sensorController.GetFanSpeedsAsync().ConfigureAwait(false);

        SetValue(0, CPU_SENSOR_NAME, cpuFanSpeed);
        SetValue(1, GPU_SENSOR_NAME, gpuFanSpeed);
    }

    private static void SetValue(int index, string name, int value)
    {
        Registry.SetValue(CUSTOM_SENSOR_HIVE,
            $@"{CUSTOM_SENSOR_PATH}\{CUSTOM_SENSOR_GROUP_NAME}\Fan{index}",
            "Name",
            name);
        Registry.SetValue(CUSTOM_SENSOR_HIVE,
            $@"{CUSTOM_SENSOR_PATH}\{CUSTOM_SENSOR_GROUP_NAME}\Fan{index}",
            "Value",
            value);
    }

    private static void ClearValues()
    {
        try
        {
            Registry.Delete(CUSTOM_SENSOR_HIVE, $@"{CUSTOM_SENSOR_PATH}\{CUSTOM_SENSOR_GROUP_NAME}");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to clear values.", ex);
        }
    }
}
