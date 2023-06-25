using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers.Sensors;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public partial class SensorsControl
{
    private readonly ISensorsController _controller = IoCContainer.Resolve<ISensorsController>();
    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

    private CancellationTokenSource? _cts;
    private Task? _refreshTask;

    public SensorsControl()
    {
        InitializeComponent();
        IsVisibleChanged += BatteryPage_IsVisibleChanged;
    }

    private async void BatteryPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
        {
            Refresh();
            return;
        }

        _cts?.Cancel();
        if (_refreshTask is not null)
            await _refreshTask;
        _refreshTask = null;
    }

    private void Refresh()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        var token = _cts.Token;

        _refreshTask = Task.Run(async () =>
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Sensors refresh started...");

            if (!await _controller.IsSupportedAsync())
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Sensors not supported.");

                Visibility = Visibility.Collapsed;
                return;
            }

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var data = await _controller.GetDataAsync();

                    Dispatcher.Invoke(() =>
                    {
                        _cpuTemperatureBar.Minimum = 0;
                        _cpuTemperatureBar.Maximum = data.CPU.MaxTemperature;
                        _cpuTemperatureBar.Value = data.CPU.CurrentTemperature;
                        _cpuTemperatureLabel.Content = GetTemperatureText(data.CPU.CurrentTemperature);
                        _cpuTemperatureLabel.Tag = (double)data.CPU.CurrentTemperature;

                        _cpuFanSpeedBar.Minimum = 0;
                        _cpuFanSpeedBar.Maximum = data.CPU.MaxFanSpeed;
                        _cpuFanSpeedBar.Value = data.CPU.CurrentFanSpeed;
                        _cpuFanSpeedLabel.Content = $"{data.CPU.CurrentFanSpeed} RPM";

                        var gpuCoreClockAvailable = data.GPU.CoreClock > 0 || data.GPU.MaxCoreClock > 0;
                        _gpuCoreClockBar.Minimum = 0;
                        _gpuCoreClockBar.Maximum = gpuCoreClockAvailable ? data.GPU.MaxCoreClock : 0;
                        _gpuCoreClockBar.Value = gpuCoreClockAvailable ? data.GPU.CoreClock : 0;
                        _gpuCoreClockLabel.Content = gpuCoreClockAvailable ? $"{data.GPU.CoreClock} MHz" : "-";

                        var gpuMemoryClockAvailable = data.GPU.MemoryClock > 0 || data.GPU.MaxMemoryClock > 0;
                        _gpuMemoryClockBar.Minimum = 0;
                        _gpuMemoryClockBar.Maximum = gpuMemoryClockAvailable ? data.GPU.MaxMemoryClock : 0;
                        _gpuMemoryClockBar.Value = gpuMemoryClockAvailable ? data.GPU.MemoryClock : 0;
                        _gpuMemoryClockLabel.Content = gpuMemoryClockAvailable ? $"{data.GPU.MemoryClock} MHz" : "-";

                        _gpuTemperatureBar.Minimum = 0;
                        _gpuTemperatureBar.Maximum = data.GPU.MaxTemperature; ;
                        _gpuTemperatureBar.Value = data.GPU.CurrentTemperature;
                        _gpuTemperatureLabel.Content = GetTemperatureText(data.GPU.CurrentTemperature);
                        _gpuTemperatureLabel.Tag = (double)data.GPU.CurrentTemperature;

                        _gpuFanSpeedBar.Minimum = 0;
                        _gpuFanSpeedBar.Maximum = data.GPU.MaxFanSpeed;
                        _gpuFanSpeedBar.Value = data.GPU.CurrentFanSpeed;
                        _gpuFanSpeedLabel.Content = $"{data.GPU.CurrentFanSpeed} RPM";
                    });

                    await Task.Delay(TimeSpan.FromSeconds(2), token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Sensors refresh failed.", ex);
                }
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Sensors refresh stopped.");
        }, token);
    }

    private void TemperatureLabel_Click(object sender, RoutedEventArgs e)
    {
        _settings.Store.TemperatureUnit = _settings.Store.TemperatureUnit == TemperatureUnit.C ? TemperatureUnit.F : TemperatureUnit.C;
        _settings.SynchronizeStore();

        if (_cpuTemperatureLabel.Tag is double cpuTemperature)
            _cpuTemperatureLabel.Content = GetTemperatureText(cpuTemperature);
        if (_gpuTemperatureLabel.Tag is double gpuTemperature)
            _gpuTemperatureLabel.Content = GetTemperatureText(gpuTemperature);
    }

    private string GetTemperatureText(double temperature)
    {
        if (_settings.Store.TemperatureUnit == TemperatureUnit.F)
        {
            temperature *= 9.0 / 5.0;
            temperature += 32;
            return $"{temperature:0} {Resource.Fahrenheit}";
        }

        return $"{temperature:0} {Resource.Celsius}";
    }
}
