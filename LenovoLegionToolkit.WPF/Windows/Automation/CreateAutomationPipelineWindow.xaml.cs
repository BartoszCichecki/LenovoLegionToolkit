using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Extensions;
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
        new GamesAreRunningAutomationPipelineTrigger(),
        new GamesStopAutomationPipelineTrigger(),
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

    private bool _multiSelect;

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

    private void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        var triggers = _content.Children.ToArray()
            .OfType<CardControl>()
            .Select(c => c.Header)
            .OfType<CardHeaderControl>()
            .Select(c => c.Accessory)
            .OfType<CheckBox>()
            .Where(c => c.IsChecked ?? false)
            .Select(c => c.Tag)
            .OfType<IAutomationPipelineTrigger>()
            .ToArray();

        if (triggers.IsEmpty())
            return;

        var trigger = triggers.Length == 1 ? triggers[0] : new AndAutomationPipelineTrigger(triggers);
        _createPipeline(trigger);

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private Task RefreshAsync()
    {
        _content.Children.Clear();

        if (!_multiSelect)
            _content.Children.Add(CreateMultipleSelectCardControl());

        foreach (var trigger in _triggers)
            _content.Children.Add(CreateCardControl(trigger));

        _createButton.IsEnabled = false;
        _createButton.Visibility = _multiSelect ? Visibility.Visible : Visibility.Collapsed;

        return Task.CompletedTask;
    }

    private UIElement CreateMultipleSelectCardControl()
    {
        var control = new CardControl
        {
            Icon = SymbolRegular.SquareMultiple24,
            Header = new CardHeaderControl
            {
                Title = "Multiple triggers...",
                Accessory = new SymbolIcon { Symbol = SymbolRegular.ChevronRight24 }
            },
            Margin = new(0, 8, 0, 0),
        };

        control.Click += async (_, _) =>
        {
            _multiSelect = true;
            await RefreshAsync();
        };

        return control;
    }

    private UIElement CreateCardControl(IAutomationPipelineTrigger trigger)
    {
        UIElement accessory;

        if (_multiSelect)
        {
            var checkbox = new CheckBox
            {
                Tag = trigger,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            checkbox.Click += (_, e) =>
            {
                RefreshCreateButton();
                e.Handled = true;
            };
            accessory = checkbox;
        }
        else
        {
            accessory = new SymbolIcon { Symbol = SymbolRegular.ChevronRight24 };
        }

        var control = new CardControl
        {
            Icon = IconForTrigger(trigger),
            Header = new CardHeaderControl
            {
                Title = trigger.DisplayName,
                Accessory = accessory
            },
            Margin = new(0, 8, 0, 0),
        };

        if (!_multiSelect && trigger is IDisallowDuplicatesAutomationPipelineTrigger)
            control.IsEnabled = !_existingTriggerTypes.Contains(trigger.GetType());

        control.Click += (_, _) =>
        {
            if (_multiSelect)
            {
                if (accessory is not CheckBox checkbox)
                    return;

                var isChecked = checkbox.IsChecked ?? false;
                checkbox.IsChecked = !isChecked;
                RefreshCreateButton();
            }
            else
            {
                _createPipeline(trigger);
                Close();
            }
        };

        return control;
    }

    private void RefreshCreateButton()
    {
        if (!_multiSelect)
        {
            _createButton.IsEnabled = false;
            return;
        }

        var anyChecked = _content.Children.ToArray()
            .OfType<CardControl>()
            .Select(c => c.Header)
            .OfType<CardHeaderControl>()
            .Select(c => c.Accessory)
            .OfType<CheckBox>()
            .Any(c => c.IsChecked ?? false);

        _createButton.IsEnabled = anyChecked;
    }

    private static SymbolRegular IconForTrigger(IAutomationPipelineTrigger trigger) => trigger switch
    {
        IPowerStateAutomationPipelineTrigger => SymbolRegular.BatteryCharge24,
        IPowerModeAutomationPipelineTrigger => SymbolRegular.Gauge24,
        IGameAutomationPipelineTrigger => SymbolRegular.XboxController24,
        IProcessesAutomationPipelineTrigger => SymbolRegular.WindowConsole20,
        ITimeAutomationPipelineTrigger => SymbolRegular.HourglassHalf24,
        INativeWindowsMessagePipelineTrigger => SymbolRegular.Desktop24,
        IOnStartupAutomationPipelineTrigger => SymbolRegular.Flash24,
        _ => throw new ArgumentException(nameof(trigger))
    };
}