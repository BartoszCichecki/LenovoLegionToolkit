using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Windows.Dashboard;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class PowerModeControl : AbstractComboBoxFeatureCardControl<PowerModeState>
{
    private readonly ThermalModeListener _thermalModeListener = IoCContainer.Resolve<ThermalModeListener>();
    private readonly PowerModeListener _powerModeListener = IoCContainer.Resolve<PowerModeListener>();
    private readonly PowerPlanListener _powerPlanListener = IoCContainer.Resolve<PowerPlanListener>();

    private readonly ThrottleFirstDispatcher _throttleDispatcher = new(TimeSpan.FromSeconds(1), nameof(PowerModeControl));

    private readonly Button _configButton = new()
    {
        Icon = SymbolRegular.Settings24,
        FontSize = 20,
        Margin = new(8, 0, 0, 0),
        Visibility = Visibility.Collapsed,
    };

    public PowerModeControl()
    {
        Icon = SymbolRegular.Gauge24;
        Title = Resource.PowerModeControl_Title;
        Subtitle = Resource.PowerModeControl_Message;

        _thermalModeListener.Changed += ThermalModeListener_Changed;
        _powerModeListener.Changed += PowerModeListener_Changed;
        _powerPlanListener.Changed += PowerPlanListener_Changed;
    }

    private void ThermalModeListener_Changed(object? sender, ThermalModeState e) => Dispatcher.Invoke(() => _throttleDispatcher.DispatchAsync(async () =>
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    }));

    private void PowerModeListener_Changed(object? sender, PowerModeState e) => Dispatcher.Invoke(() => _throttleDispatcher.DispatchAsync(async () =>
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    }));

    private void PowerPlanListener_Changed(object? sender, EventArgs e) => Dispatcher.Invoke(() => _throttleDispatcher.DispatchAsync(async () =>
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    }));

    protected override async Task OnRefreshAsync()
    {
        await base.OnRefreshAsync();

        if (await Power.IsPowerAdapterConnectedAsync() != PowerAdapterStatus.Connected
            && TryGetSelectedItem(out var state)
            && state is PowerModeState.Performance or PowerModeState.GodMode)
            Warning = Resource.PowerModeControl_Warning;
        else
            Warning = string.Empty;
    }

    protected override async Task OnStateChange(ComboBox comboBox, IFeature<PowerModeState> feature, PowerModeState? newValue, PowerModeState? oldValue)
    {
        await base.OnStateChange(comboBox, feature, newValue, oldValue);

        var mi = await Compatibility.GetMachineInformationAsync();

        switch (newValue)
        {
            case PowerModeState.Balance when mi.Properties.SupportsIntelligentSubMode:
                _configButton.ToolTip = Resource.PowerModeControl_Settings;
                _configButton.Visibility = Visibility.Visible;
                break;
            case PowerModeState.GodMode when mi.Properties.SupportsGodMode:
                _configButton.ToolTip = Resource.PowerModeControl_Settings;
                _configButton.Visibility = Visibility.Visible;
                break;
            default:
                _configButton.ToolTip = null;
                _configButton.Visibility = Visibility.Collapsed;
                break;
        }
    }

    protected override FrameworkElement GetAccessory(ComboBox comboBox)
    {
        _configButton.Click += ConfigButton_Click;

        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };
        stackPanel.Children.Add(_configButton);
        stackPanel.Children.Add(comboBox);

        return stackPanel;
    }

    private void ConfigButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetSelectedItem(out var state))
            return;

        switch (state)
        {
            case PowerModeState.Balance:
                {
                    var window = new BalanceModeSettingsWindow { Owner = Window.GetWindow(this) };
                    window.ShowDialog();
                    break;
                }
            case PowerModeState.GodMode:
                {
                    var window = new GodModeSettingsWindow { Owner = Window.GetWindow(this) };
                    window.ShowDialog();
                    break;
                }
        }
    }
}