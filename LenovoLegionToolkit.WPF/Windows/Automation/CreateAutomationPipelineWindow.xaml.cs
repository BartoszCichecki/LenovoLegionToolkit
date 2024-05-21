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
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using CardControl = LenovoLegionToolkit.WPF.Controls.Custom.CardControl;

namespace LenovoLegionToolkit.WPF.Windows.Automation;

public partial class CreateAutomationPipelineWindow
{
    private readonly IAutomationPipelineTrigger[] _triggers =
    [
        new ACAdapterConnectedAutomationPipelineTrigger(),
        new LowWattageACAdapterConnectedAutomationPipelineTrigger(),
        new ACAdapterDisconnectedAutomationPipelineTrigger(),
        new PowerModeAutomationPipelineTrigger(PowerModeState.Balance),
        new GodModePresetChangedAutomationPipelineTrigger(Guid.Empty),
        new GamesAreRunningAutomationPipelineTrigger(),
        new GamesStopAutomationPipelineTrigger(),
        new ProcessesAreRunningAutomationPipelineTrigger([]),
        new ProcessesStopRunningAutomationPipelineTrigger([]),
        new UserInactivityAutomationPipelineTrigger(TimeSpan.Zero),
        new UserInactivityAutomationPipelineTrigger(TimeSpan.FromMinutes(1)),
        new LidOpenedAutomationPipelineTrigger(),
        new LidClosedAutomationPipelineTrigger(),
        new DisplayOnAutomationPipelineTrigger(),
        new DisplayOffAutomationPipelineTrigger(),
        new HDROnAutomationPipelineTrigger(),
        new HDROffAutomationPipelineTrigger(),
        new DeviceConnectedAutomationPipelineTrigger([]),
        new DeviceDisconnectedAutomationPipelineTrigger([]),
        new ExternalDisplayConnectedAutomationPipelineTrigger(),
        new ExternalDisplayDisconnectedAutomationPipelineTrigger(),
        new WiFiConnectedAutomationPipelineTrigger([]),
        new WiFiDisconnectedAutomationPipelineTrigger(),
        new TimeAutomationPipelineTrigger(false, false, TimeExtensions.UtcNow, Enum.GetValues<DayOfWeek>()),
        new PeriodicAutomationPipelineTrigger(TimeSpan.FromMinutes(1)),
        new OnStartupAutomationPipelineTrigger(),
        new OnResumeAutomationPipelineTrigger()
    ];

    private readonly HashSet<Type> _existingTriggerTypes;
    private readonly Action<IAutomationPipelineTrigger> _createPipeline;

    private bool _multiSelect;

    public CreateAutomationPipelineWindow(HashSet<Type> existingTriggerTypes,
        Action<IAutomationPipelineTrigger> createPipeline)
    {
        _existingTriggerTypes = existingTriggerTypes;
        _createPipeline = createPipeline;

        InitializeComponent();

        IsVisibleChanged += CreateAutomationPipelineWindow_IsVisibleChanged;
    }

    private async void CreateAutomationPipelineWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
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

    private CardControl CreateMultipleSelectCardControl()
    {
        var control = new CardControl
        {
            Icon = SymbolRegular.SquareMultiple24,
            Header = new CardHeaderControl
            {
                Title = Resource.MultipleTriggersAutomationPipelineTrigger_DisplayName,
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

    private CardControl CreateCardControl(IAutomationPipelineTrigger trigger)
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
            Icon = trigger.Icon(),
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
}
