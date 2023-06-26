using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

                Dispatcher.Invoke(() => Visibility = Visibility.Collapsed);
                return;
            }

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var data = await _controller.GetDataAsync();

                    Dispatcher.Invoke(() =>
                    {
                        UpdateValue(_cpuUtilizationBar, _cpuUtilizationLabel, 100, data.CPU.Utilization,
                            $"{data.CPU.Utilization} %");
                        UpdateValue(_cpuCoreClockBar, _cpuCoreClockLabel, data.CPU.MaxCoreClock, data.CPU.CoreClock,
                            $"{data.CPU.CoreClock / 1000.0:0.0} GHz", $"{data.CPU.MaxCoreClock / 1000.0:0.0} GHz");
                        UpdateValue(_cpuTemperatureBar, _cpuTemperatureLabel, data.CPU.MaxTemperature, data.CPU.Temperature,
                            GetTemperatureText(data.CPU.Temperature), GetTemperatureText(data.CPU.MaxTemperature));
                        UpdateValue(_cpuFanSpeedBar, _cpuFanSpeedLabel, data.CPU.MaxFanSpeed, data.CPU.FanSpeed,
                            $"{data.CPU.FanSpeed} RPM", $"{data.CPU.MaxFanSpeed} RPM");

                        UpdateValue(_gpuUtilizationBar, _gpuUtilizationLabel, 100, data.GPU.Utilization,
                            $"{data.GPU.Utilization} %");
                        UpdateValue(_gpuCoreClockBar, _gpuCoreClockLabel, data.GPU.MaxCoreClock, data.GPU.CoreClock,
                            $"{data.GPU.CoreClock} MHz", $"{data.GPU.MaxCoreClock} MHz");
                        UpdateValue(_gpuMemoryClockBar, _gpuMemoryClockLabel, data.GPU.MaxMemoryClock, data.GPU.MemoryClock,
                            $"{data.GPU.MemoryClock} MHz", $"{data.GPU.MaxMemoryClock} MHz");
                        UpdateValue(_gpuTemperatureBar, _gpuTemperatureLabel, data.GPU.MaxTemperature, data.GPU.Temperature,
                            GetTemperatureText(data.GPU.Temperature), GetTemperatureText(data.GPU.MaxTemperature));
                        UpdateValue(_gpuFanSpeedBar, _gpuFanSpeedLabel, data.GPU.MaxFanSpeed, data.GPU.FanSpeed,
                            $"{data.GPU.FanSpeed} RPM", $"{data.GPU.MaxFanSpeed} RPM");
                    });

                    await Task.Delay(TimeSpan.FromSeconds(2), token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Sensors refresh failed.", ex);

                    Dispatcher.Invoke(() => Visibility = Visibility.Collapsed);

                    return;
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

    private static void UpdateValue(RangeBase bar, ContentControl label, double max, double value, string text, string? toolTipText = null)
    {
        if (max < 0 || value < 0)
        {
            bar.Minimum = 0;
            bar.Maximum = 1;
            bar.Value = 0;
            label.Content = "-";
            label.ToolTip = null;
            label.Tag = 0;
        }
        else
        {
            bar.Minimum = 0;
            bar.Maximum = max;
            bar.Value = value;
            label.Content = text;
            label.ToolTip = toolTipText is null ? null : $"Maximum: {toolTipText}";
            label.Tag = value;
        }
    }
}
