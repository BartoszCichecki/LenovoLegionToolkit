using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Humanizer;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers.Sensors;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Settings;
using Wpf.Ui.Common;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public partial class SensorsControl
{
    private readonly ISensorsController _controller = IoCContainer.Resolve<ISensorsController>();
    private readonly ApplicationSettings _applicationSettings = IoCContainer.Resolve<ApplicationSettings>();
    private readonly DashboardSettings _dashboardSettings = IoCContainer.Resolve<DashboardSettings>();

    private CancellationTokenSource? _cts;
    private Task? _refreshTask;

    public SensorsControl()
    {
        InitializeComponent();
        InitializeContextMenu();

        IsVisibleChanged += SensorsControl_IsVisibleChanged;
    }

    private void InitializeContextMenu()
    {
        ContextMenu = new ContextMenu();
        ContextMenu.Items.Add(new MenuItem { Header = Resource.SensorsControl_RefreshInterval, IsEnabled = false });

        foreach (var interval in new[] { 1, 2, 3, 5 })
        {
            var item = new MenuItem
            {
                SymbolIcon = _dashboardSettings.Store.SensorsRefreshIntervalSeconds == interval ? SymbolRegular.Checkmark24 : SymbolRegular.Empty,
                Header = TimeSpan.FromSeconds(interval).Humanize(culture: Resource.Culture)
            };
            item.Click += (_, _) =>
            {
                _dashboardSettings.Store.SensorsRefreshIntervalSeconds = interval;
                _dashboardSettings.SynchronizeStore();
                InitializeContextMenu();
            };
            ContextMenu.Items.Add(item);
        }
    }

    private async void SensorsControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
        {
            Refresh();
            return;
        }

        if (_cts is not null)
            await _cts.CancelAsync();

        _cts = null;

        if (_refreshTask is not null)
            await _refreshTask;

        _refreshTask = null;

        UpdateValues(SensorsData.Empty);
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

            await _controller.PrepareAsync();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var data = await _controller.GetDataAsync();
                    Dispatcher.Invoke(() => UpdateValues(data));
                    await Task.Delay(TimeSpan.FromSeconds(_dashboardSettings.Store.SensorsRefreshIntervalSeconds), token);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Sensors refresh failed.", ex);

                    Dispatcher.Invoke(() => UpdateValues(SensorsData.Empty));
                }
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Sensors refresh stopped.");
        }, token);
    }

    private void UpdateValues(SensorsData data)
    {
        UpdateValue(_cpuUtilizationBar, _cpuUtilizationLabel, data.CPU.MaxUtilization, data.CPU.Utilization,
            $"{data.CPU.Utilization}%");
        UpdateValue(_cpuCoreClockBar, _cpuCoreClockLabel, data.CPU.MaxCoreClock, data.CPU.CoreClock,
            $"{data.CPU.CoreClock / 1000.0:0.0} {Resource.GHz}", $"{data.CPU.MaxCoreClock / 1000.0:0.0} {Resource.GHz}");
        UpdateValue(_cpuTemperatureBar, _cpuTemperatureLabel, data.CPU.MaxTemperature, data.CPU.Temperature,
            GetTemperatureText(data.CPU.Temperature), GetTemperatureText(data.CPU.MaxTemperature));
        UpdateValue(_cpuFanSpeedBar, _cpuFanSpeedLabel, data.CPU.MaxFanSpeed, data.CPU.FanSpeed,
            $"{data.CPU.FanSpeed} {Resource.RPM}", $"{data.CPU.MaxFanSpeed} {Resource.RPM}");

        UpdateValue(_gpuUtilizationBar, _gpuUtilizationLabel, data.GPU.MaxUtilization, data.GPU.Utilization,
            $"{data.GPU.Utilization} %");
        UpdateValue(_gpuCoreClockBar, _gpuCoreClockLabel, data.GPU.MaxCoreClock, data.GPU.CoreClock,
            $"{data.GPU.CoreClock} {Resource.MHz}", $"{data.GPU.MaxCoreClock} {Resource.MHz}");
        UpdateValue(_gpuMemoryClockBar, _gpuMemoryClockLabel, data.GPU.MaxMemoryClock, data.GPU.MemoryClock,
            $"{data.GPU.MemoryClock} {Resource.MHz}", $"{data.GPU.MaxMemoryClock} {Resource.MHz}");
        UpdateValue(_gpuTemperatureBar, _gpuTemperatureLabel, data.GPU.MaxTemperature, data.GPU.Temperature,
            GetTemperatureText(data.GPU.Temperature), GetTemperatureText(data.GPU.MaxTemperature));
        UpdateValue(_gpuFanSpeedBar, _gpuFanSpeedLabel, data.GPU.MaxFanSpeed, data.GPU.FanSpeed,
            $"{data.GPU.FanSpeed} {Resource.RPM}", $"{data.GPU.MaxFanSpeed} {Resource.RPM}");
    }

    private string GetTemperatureText(double temperature)
    {
        if (_applicationSettings.Store.TemperatureUnit == TemperatureUnit.F)
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
            label.ToolTip = toolTipText is null ? null : string.Format(Resource.SensorsControl_Maximum, toolTipText);
            label.Tag = value;
        }
    }
}
