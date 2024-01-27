using System;
using System.Globalization;
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
    private const string SENSOR_TYPE_FAN = "Fan";
    private const string SENSOR_TYPE_TEMP = "Temp";
    private const string CPU_FAN_SENSOR_NAME = "CPU Fan";
    private const string GPU_FAN_SENSOR_NAME = "GPU Fan";
    private const string BATTERY_TEMP_SENSOR_NAME = "Battery Temperature";

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
                token.ThrowIfCancellationRequested();
                await SetSensorValuesAsync().ConfigureAwait(false);
                await Task.Delay(_refreshInterval, token).ConfigureAwait(false);

            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to set values.", ex);
        }
    }

    private async Task SetSensorValuesAsync()
    {
        var (cpuFanSpeed, gpuFanSpeed) = await _sensorController.GetFanSpeedsAsync().ConfigureAwait(false);
        var batteryTemp = Battery.GetBatteryInformation().BatteryTemperatureC;

        SetValue(SENSOR_TYPE_FAN, 0, CPU_FAN_SENSOR_NAME, cpuFanSpeed);
        SetValue(SENSOR_TYPE_FAN, 1, GPU_FAN_SENSOR_NAME, gpuFanSpeed);

        if (batteryTemp.HasValue)
        {
            var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
            SetValue(SENSOR_TYPE_TEMP, 0, BATTERY_TEMP_SENSOR_NAME, batteryTemp.Value.ToString(nfi));
        }
        else
        {
            SetValue(SENSOR_TYPE_TEMP, 0, BATTERY_TEMP_SENSOR_NAME, string.Empty);
        }
    }

    private static void SetValue<T>(string type, int index, string name, T value) where T : notnull
    {
        Registry.SetValue(CUSTOM_SENSOR_HIVE,
            $@"{CUSTOM_SENSOR_PATH}\{CUSTOM_SENSOR_GROUP_NAME}\{type}{index}",
            "Name",
            name);
        Registry.SetValue(CUSTOM_SENSOR_HIVE,
            $@"{CUSTOM_SENSOR_PATH}\{CUSTOM_SENSOR_GROUP_NAME}\{type}{index}",
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
