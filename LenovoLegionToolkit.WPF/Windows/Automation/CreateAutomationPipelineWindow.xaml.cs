using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.WPF.Controls;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Automation;

public partial class CreateAutomationPipelineWindow
{
    private readonly IAutomationPipelineTrigger[] _triggers =
    {
        new ACAdapterConnectedAutomationPipelineTrigger(),
        new LowWattageACAdapterConnectedAutomationPipelineTrigger(),
        new ACAdapterDisconnectedAutomationPipelineTrigger(),
        new PowerModeAutomationPipelineTrigger(PowerModeState.Balance),
        new ProcessesAreRunningAutomationPipelineTrigger(Array.Empty<ProcessInfo>()),
        new ProcessesStopRunningAutomationPipelineTrigger(Array.Empty<ProcessInfo>()),
        new LidOpenedAutomationPipelineTrigger(),
        new LidClosedAutomationPipelineTrigger(),
        new DisplayOnAutomationPipelineTrigger(),
        new DisplayOffAutomationPipelineTrigger(),
        new ExternalDisplayConnectedAutomationPipelineTrigger(),
        new ExternalDisplayDisconnectedAutomationPipelineTrigger(),
        new TimeAutomationPipelineTrigger(false, false, null),
        new OnStartupAutomationPipelineTrigger()
    };

    private readonly HashSet<Type> _existingTriggerTypes;
    private readonly Action<IAutomationPipelineTrigger> _createPipeline;

    public CreateAutomationPipelineWindow(HashSet<Type> existingTriggerTypes, Action<IAutomationPipelineTrigger> createPipeline)
    {
        _existingTriggerTypes = existingTriggerTypes;
        _createPipeline = createPipeline;

        InitializeComponent();
    }

    private async void CreateAutomationPipelineWindow_Loaded(object _1, RoutedEventArgs _2) => await RefreshAsync();

    private async void CreateAutomationPipelineWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private Task RefreshAsync()
    {
        _content.Children.Clear();

        foreach (var trigger in _triggers)
            _content.Children.Add(CreateCardControl(trigger));

        return Task.CompletedTask;
    }

    private UIElement CreateCardControl(IAutomationPipelineTrigger trigger)
    {
        var control = new CardControl
        {
            Icon = IconForTrigger(trigger),
            Header = new CardHeaderControl
            {
                Title = trigger.DisplayName,
                Accessory = new SymbolIcon { Symbol = SymbolRegular.ChevronRight24 }
            },
            Margin = new(0, 8, 0, 0),
        };

        if (trigger is IDisallowDuplicatesAutomationPipelineTrigger)
            control.IsEnabled = !_existingTriggerTypes.Contains(trigger.GetType());

        control.Click += (_, _) =>
        {
            _createPipeline(trigger);
            Close();
        };

        return control;
    }

    private static SymbolRegular IconForTrigger(IAutomationPipelineTrigger trigger) => trigger switch
    {
        IPowerStateAutomationPipelineTrigger => SymbolRegular.BatteryCharge24,
        IPowerModeAutomationPipelineTrigger => SymbolRegular.Gauge24,
        IProcessesAutomationPipelineTrigger => SymbolRegular.WindowConsole20,
        ITimeAutomationPipelineTrigger => SymbolRegular.HourglassHalf24,
        INativeWindowsMessagePipelineTrigger => SymbolRegular.Desktop24,
        IOnStartupAutomationPipelineTrigger => SymbolRegular.Flash24,
        _ => throw new ArgumentException(nameof(trigger))
    };
}