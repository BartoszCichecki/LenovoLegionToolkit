using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers.Sensors;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Integrations;

public class HWiNFOIntegration(SensorsController sensorController, IntegrationsSettings settings)
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

    private CancellationTokenSource? _cts;
    private Task? _refreshTask;

    public async Task StartStopIfNeededAsync()
    {
        await StopAsync().ConfigureAwait(false);

        if (!settings.Store.HWiNFO)
            return;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting...");

        _cts = new();
        _refreshTask = RefreshLoopAsync(_cts.Token);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Started.");
    }

    public async Task StopAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopping...");

        if (_cts is not null)
            await _cts.CancelAsync().ConfigureAwait(false);

        if (_refreshTask is not null)
            await _refreshTask.ConfigureAwait(false);

        ClearValues();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopped.");
    }

    private async Task RefreshLoopAsync(CancellationToken token)
    {
        try
        {
            await SetSensorValuesAsync().ConfigureAwait(false);

            while (true)
            {
                await Task.Delay(_refreshInterval, token).ConfigureAwait(false);
                await SetSensorValuesAsync(false).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to set values.", ex);
        }
    }

    private async Task SetSensorValuesAsync(bool firstRun = true)
    {
        var (cpuFanSpeed, gpuFanSpeed) = await sensorController.GetFanSpeedsAsync().ConfigureAwait(false);
        var batteryTemp = Battery.GetBatteryTemperatureC();

        SetValue(SENSOR_TYPE_FAN, 0, CPU_FAN_SENSOR_NAME, cpuFanSpeed, firstRun);
        SetValue(SENSOR_TYPE_FAN, 1, GPU_FAN_SENSOR_NAME, gpuFanSpeed, firstRun);

        var batteryTempString = batteryTemp.HasValue
            ? batteryTemp.Value.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." })
            : string.Empty;
        SetValue(SENSOR_TYPE_TEMP, 0, BATTERY_TEMP_SENSOR_NAME, batteryTempString, firstRun);
    }

    private static void SetValue<T>(string type, int index, string name, T value, bool firstRun) where T : notnull
    {
        Registry.SetValue(CUSTOM_SENSOR_HIVE,
            $@"{CUSTOM_SENSOR_PATH}\{CUSTOM_SENSOR_GROUP_NAME}\{type}{index}",
            "Value",
            value);

        if (!firstRun)
            return;

        Registry.SetValue(CUSTOM_SENSOR_HIVE,
            $@"{CUSTOM_SENSOR_PATH}\{CUSTOM_SENSOR_GROUP_NAME}\{type}{index}",
            "Name",
            name);
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
