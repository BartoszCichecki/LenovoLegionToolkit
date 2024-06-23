﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Humanizer;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Controls.Automation.Steps;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Automation;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;
using CardExpander = LenovoLegionToolkit.WPF.Controls.Custom.CardExpander;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Controls.Automation;

public class AutomationPipelineControl : UserControl
{
    private readonly TaskCompletionSource _initializedTaskCompletionSource = new();

    private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();
    private readonly GodModeSettings _godModeSettings = IoCContainer.Resolve<GodModeSettings>();

    private readonly CardExpander _cardExpander = new()
    {
        Margin = new(0, 0, 0, 8),
    };

    private readonly CardHeaderControl _cardHeaderControl = new();

    private readonly StackPanel _stackPanel = new();

    private readonly StackPanel _stepsStackPanel = new();

    private readonly Grid _buttonsStackPanel = new()
    {
        Margin = new(0, 16, 0, 0),
        ColumnDefinitions =
        {
            new() { Width = new(1, GridUnitType.Star) },
            new() { Width = GridLength.Auto },
            new() { Width = GridLength.Auto },
            new() { Width = GridLength.Auto },
        }
    };

    private readonly CheckBox _isExclusiveCheckBox = new()
    {
        HorizontalAlignment = HorizontalAlignment.Left,
        Content = Resource.AutomationPipelineControl_Exclusive,
        ToolTip = Resource.AutomationPipelineControl_Exclusive_ToolTip,
        MinWidth = 100,
        Margin = new(0, 0, 8, 0),
    };

    private readonly Button _runNowButton = new()
    {
        Content = Resource.AutomationPipelineControl_RunNow,
        MinWidth = 100,
        Margin = new(0, 0, 8, 0),
    };

    private readonly Button _addStepButton = new()
    {
        Content = Resource.AutomationPipelineControl_AddStep,
        MinWidth = 100,
        Margin = new(0, 0, 8, 0),
    };

    private readonly Button _deletePipelineButton = new()
    {
        Content = Resource.Delete,
        MinWidth = 100,
    };

    private readonly IAutomationStep[] _supportedAutomationSteps;

    public AutomationPipeline AutomationPipeline { get; }

    public event EventHandler? OnChanged;
    public event EventHandler? OnDelete;

    public Task InitializedTask => _initializedTaskCompletionSource.Task;

    public AutomationPipelineControl(AutomationPipeline automationPipeline, IAutomationStep[] supportedAutomationSteps)
    {
        AutomationPipeline = automationPipeline;
        _supportedAutomationSteps = supportedAutomationSteps;

        Initialized += AutomationPipelineControl_Initialized;
    }

    public AutomationPipeline CreateAutomationPipeline() => new()
    {
        Id = AutomationPipeline.Id,
        IconName = AutomationPipeline.IconName,
        Name = AutomationPipeline.Name,
        Trigger = AutomationPipeline.Trigger,
        Steps = _stepsStackPanel.Children.ToArray()
            .OfType<AbstractAutomationStepControl>()
            .Select(s => s.CreateAutomationStep())
            .ToList(),
        IsExclusive = _isExclusiveCheckBox.IsChecked ?? false,
    };

    public string? GetName() => AutomationPipeline.Name;

    public void SetName(string? name)
    {
        AutomationPipeline.Name = name;
        _cardHeaderControl.Title = GenerateHeader();
        _cardHeaderControl.Subtitle = GenerateSubtitle();
        _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;

        OnChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetIcon(SymbolRegular? icon)
    {
        AutomationPipeline.IconName = icon.HasValue ? Enum.GetName(icon.Value) : null;
        _cardExpander.Icon = GenerateIcon();

        OnChanged?.Invoke(this, EventArgs.Empty);
    }

    private async void AutomationPipelineControl_Initialized(object? sender, EventArgs e)
    {
        _cardExpander.Header = _cardHeaderControl;

        foreach (var step in AutomationPipeline.Steps)
        {
            var control = await GenerateStepControlAsync(step);
            _stepsStackPanel.Children.Add(control);
        }

        if (AutomationPipeline.Trigger is not null)
        {
            _isExclusiveCheckBox.IsChecked = AutomationPipeline.IsExclusive;
            _isExclusiveCheckBox.Checked += (_, _) => OnChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _isExclusiveCheckBox.Visibility = Visibility.Hidden;
        }

        _runNowButton.Click += async (_, _) => await RunAsync();

        _addStepButton.Click += async (_, _) =>
        {
            var stepControls = new List<AbstractAutomationStepControl>();
            foreach (var step in _supportedAutomationSteps)
                stepControls.Add(await GenerateStepControlAsync(step));

            var window = new AddAutomationStepWindow(stepControls, AddStep) { Owner = Window.GetWindow(this) };
            window.ShowDialog();
        };

        _deletePipelineButton.Click += (_, _) => OnDelete?.Invoke(this, EventArgs.Empty);

        Grid.SetColumn(_isExclusiveCheckBox, 0);
        Grid.SetColumn(_runNowButton, 1);
        Grid.SetColumn(_addStepButton, 2);
        Grid.SetColumn(_deletePipelineButton, 3);

        _buttonsStackPanel.Children.Add(_isExclusiveCheckBox);
        _buttonsStackPanel.Children.Add(_runNowButton);
        _buttonsStackPanel.Children.Add(_addStepButton);
        _buttonsStackPanel.Children.Add(_deletePipelineButton);

        _stackPanel.Children.Add(_stepsStackPanel);
        _stackPanel.Children.Add(_buttonsStackPanel);

        _cardExpander.Icon = GenerateIcon();
        _cardHeaderControl.Title = GenerateHeader();
        _cardHeaderControl.Subtitle = GenerateSubtitle();
        _cardHeaderControl.Accessory = GenerateAccessory();
        _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;
        _cardExpander.Content = _stackPanel;

        Content = _cardExpander;

        _initializedTaskCompletionSource.TrySetResult();
    }

    private async Task RunAsync()
    {
        try
        {
            _runNowButton.IsEnabled = false;
            _runNowButton.Content = Resource.AutomationPipelineControl_Running;
            var pipeline = CreateAutomationPipeline();
            await _automationProcessor.RunNowAsync(pipeline);

            await SnackbarHelper.ShowAsync(Resource.AutomationPipelineControl_RunNow_Success_Title, Resource.AutomationPipelineControl_RunNow_Success_Message);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Run now completed with errors", ex);

            await SnackbarHelper.ShowAsync(Resource.AutomationPipelineControl_RunNow_Error_Title, Resource.AutomationPipelineControl_RunNow_Error_Message);
        }
        finally
        {
            _runNowButton.Content = Resource.AutomationPipelineControl_RunNow;
            _runNowButton.IsEnabled = true;
        }
    }

    private SymbolRegular GenerateIcon()
    {
        if (AutomationPipeline.Trigger is not null)
            return SymbolRegular.Flow20;

        return Enum.TryParse<SymbolRegular>(AutomationPipeline.IconName, out var icon) ? icon : SymbolRegular.Play24;
    }

    private string GenerateHeader()
    {
        if (!string.IsNullOrWhiteSpace(AutomationPipeline.Name))
            return AutomationPipeline.Name;

        if (AutomationPipeline.Trigger is not null)
            return AutomationPipeline.Trigger.DisplayName;

        return Resource.AutomationPipelineControl_Unnamed;
    }

    private string GenerateSubtitle()
    {
        var stepsCount = _stepsStackPanel.Children.ToArray()
            .OfType<AbstractAutomationStepControl>()
            .Count();

        var result = string.Format(stepsCount == 1 ? Resource.AutomationPipelineControl_Step : Resource.AutomationPipelineControl_Step_Many, stepsCount);

        if (!string.IsNullOrWhiteSpace(AutomationPipeline.Name) && AutomationPipeline.Trigger is not null)
            result += $" | {AutomationPipeline.Trigger.DisplayName}";

        if (AutomationPipeline.Trigger is IPowerModeAutomationPipelineTrigger pm)
            result += $" | {Resource.AutomationPipelineControl_SubtitlePart_PowerMode}: {pm.PowerModeState.GetDisplayName()}";

        if (AutomationPipeline.Trigger is IGodModePresetChangedAutomationPipelineTrigger gpt)
        {
            var name = _godModeSettings.Store.Presets.Where(kv => kv.Key == gpt.PresetId)
                .Select(kv => kv.Value.Name)
                .DefaultIfEmpty("-")
                .First();
            result += $" | {Resource.AutomationPipelineControl_SubtitlePart_Preset}: {name}";
        }

        if (AutomationPipeline.Trigger is IProcessesAutomationPipelineTrigger pt && pt.Processes.Length != 0)
            result += $" | {Resource.AutomationPipelineControl_SubtitlePart_Apps}: {string.Join(", ", pt.Processes.Select(p => p.Name))}";

        if (AutomationPipeline.Trigger is ITimeAutomationPipelineTrigger tt)
        {
            if (tt.IsSunrise)
                result += $" | {Resource.AutomationPipelineControl_SubtitlePart_AtSunrise}";
            if (tt.IsSunset)
                result += $" | {Resource.AutomationPipelineControl_SubtitlePart_AtSunset}";
            if (tt.Time is not null)
            {
                var local = DateTimeExtensions.UtcFrom(tt.Time.Value.Hour, tt.Time.Value.Minute).ToLocalTime();
                if (tt.Days.IsEmpty() || tt.Days.OrderBy(x => x).SequenceEqual(Enum.GetValues<DayOfWeek>()))
                {
                    result += $" | {string.Format(Resource.AutomationPipelineControl_SubtitlePart_AtTime, local.Hour, local.Minute)}";
                }
                else
                {
                    var localizedDayStrings = tt.Days.Select(day => Resource.Culture.DateTimeFormat.GetDayName(day));
                    result += $" | {string.Join(", ", localizedDayStrings)} {string.Format(Resource.AutomationPipelineControl_SubtitlePart_AtTime.ToLower(Resource.Culture), local.Hour, local.Minute)}";
                }
            }
        }

        if (AutomationPipeline.Trigger is IUserInactivityPipelineTrigger ut && ut.InactivityTimeSpan > TimeSpan.Zero)
            result += $" | {string.Format(Resource.AutomationPipelineControl_SubtitlePart_After, ut.InactivityTimeSpan.Humanize(culture: Resource.Culture))}";

        if (AutomationPipeline.Trigger is IWiFiConnectedPipelineTrigger wt && wt.Ssids.Length != 0)
            result += $" | {string.Join(",", wt.Ssids)}";

        if (AutomationPipeline.Trigger is IPeriodicAutomationPipelineTrigger pet)
            result += $" | {Resource.PeriodicActionPipelineTriggerTabItemContent_PeriodMinutes}: {pet.Period.TotalMinutes}";

        if (AutomationPipeline.Trigger is IDeviceAutomationPipelineTrigger dt && dt.InstanceIds.Length != 0)
            result += $" | {Resource.DevicePipelineTriggerTabItemContent_Devices}: {dt.InstanceIds.Length}";

        return result;
    }

    private Button? GenerateAccessory()
    {
        var triggers = AutomationPipeline.AllTriggers
            .ToArray();

        if (!AutomationPipelineTriggerConfigurationWindow.IsValid(triggers))
            return null;

        var button = new Button
        {
            Content = Resource.AutomationPipelineControl_Configure,
            Margin = new(16, 0, 16, 0),
            MinWidth = 120,
        };
        button.Click += (_, _) =>
        {
            var window = new AutomationPipelineTriggerConfigurationWindow(triggers) { Owner = Window.GetWindow(this) };
            window.OnSave += (_, e) =>
            {
                AutomationPipeline.Trigger = e;
                _cardHeaderControl.Subtitle = GenerateSubtitle();
                _cardHeaderControl.Accessory = GenerateAccessory();
                _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;
                OnChanged?.Invoke(this, EventArgs.Empty);
            };
            window.ShowDialog();
        };
        return button;
    }

    private async Task<AbstractAutomationStepControl> GenerateStepControlAsync(IAutomationStep automationStep)
    {
        AbstractAutomationStepControl control = automationStep switch
        {
            AlwaysOnUsbAutomationStep s => new AlwaysOnUsbAutomationStepControl(s),
            BatteryAutomationStep s => new BatteryAutomationStepControl(s),
            BatteryNightChargeAutomationStep s => new BatteryNightChargeAutomationStepControl(s),
            DeactivateGPUAutomationStep s => new DeactivateGPUAutomationStepControl(s),
            DelayAutomationStep s => new DelayAutomationStepControl(s),
            DisplayBrightnessAutomationStep s => new DisplayBrightnessAutomationStepControl(s),
            DpiScaleAutomationStep s => new DpiScaleAutomationStepControl(s),
            FlipToStartAutomationStep s => new FlipToStartAutomationStepControl(s),
            FnLockAutomationStep s => new FnLockAutomationStepControl(s),
            GodModePresetAutomationStep s => new GodModePresetAutomationStepControl(s),
            HDRAutomationStep s => new HDRAutomationStepControl(s),
            HybridModeAutomationStep s => await HybridModeAutomationStepControlFactory.GetControlAsync(s),
            InstantBootAutomationStep s => new InstantBootAutomationStepControl(s),
            MacroAutomationStep s => new MacroAutomationStepControl(s),
            MicrophoneAutomationStep s => new MicrophoneAutomationStepControl(s),
            NotificationAutomationStep s => new NotificationAutomationStepControl(s),
            OneLevelWhiteKeyboardBacklightAutomationStep s => new OneLevelWhiteKeyboardBacklightAutomationStepControl(s),
            OverDriveAutomationStep s => new OverDriveAutomationStepControl(s),
            OverclockDiscreteGPUAutomationStep s => new OverclockDiscreteGPUAutomationStepControl(s),
            PanelLogoBacklightAutomationStep s => new PanelLogoBacklightAutomationStepControl(s),
            PortsBacklightAutomationStep s => new PortsBacklightAutomationStepControl(s),
            PowerModeAutomationStep s => new PowerModeAutomationStepControl(s),
            RefreshRateAutomationStep s => new RefreshRateAutomationStepControl(s),
            ResolutionAutomationStep s => new ResolutionAutomationStepControl(s),
            RGBKeyboardBacklightAutomationStep s => new RGBKeyboardBacklightAutomationStepControl(s),
            RunAutomationStep s => new RunAutomationStepControl(s),
            SpeakerAutomationStep s => new SpeakerAutomationStepControl(s),
            SpectrumKeyboardBacklightBrightnessAutomationStep s => new SpectrumKeyboardBacklightBrightnessAutomationStepControl(s),
            SpectrumKeyboardBacklightImportProfileAutomationStep s => new SpectrumKeyboardBacklightImportProfileAutomationStepControl(s),
            SpectrumKeyboardBacklightProfileAutomationStep s => new SpectrumKeyboardBacklightProfileAutomationStepControl(s),
            TurnOffMonitorsAutomationStep s => new TurnOffMonitorsAutomationStepControl(s),
            TurnOffWiFiAutomationStep s => new TurnOffWiFiAutomationStepControl(s),
            TurnOnWiFiAutomationStep s => new TurnOnWiFiAutomationStepControl(s),
            TouchpadLockAutomationStep s => new TouchpadLockAutomationStepControl(s),
            WhiteKeyboardBacklightAutomationStep s => new WhiteKeyboardBacklightAutomationStepControl(s),
            WinKeyAutomationStep s => new WinKeyAutomationStepControl(s),
            _ => throw new InvalidOperationException("Unknown step type"),
        };
        control.MouseRightButtonUp += (_, e) =>
        {
            ShowContextMenu(control);
            e.Handled = true;
        };
        control.Changed += (_, _) =>
        {
            OnChanged?.Invoke(this, EventArgs.Empty);
        };
        control.Delete += (s, _) =>
        {
            if (s is AbstractAutomationStepControl step)
                DeleteStep(step);
        };
        return control;
    }

    private void ShowContextMenu(FrameworkElement control)
    {
        var menuItems = new List<MenuItem>();

        var index = _stepsStackPanel.Children.IndexOf(control);
        var maxIndex = _stepsStackPanel.Children.Count - 1;

        var moveUpMenuItem = new MenuItem
        {
            SymbolIcon = SymbolRegular.ArrowUp24,
            Header = Resource.MoveUp
        };
        if (index > 0)
            moveUpMenuItem.Click += (_, _) => MoveStep(control, index - 1);
        else
            moveUpMenuItem.IsEnabled = false;
        menuItems.Add(moveUpMenuItem);

        var moveDownMenuItem = new MenuItem
        {
            SymbolIcon = SymbolRegular.ArrowDown24,
            Header = Resource.MoveDown
        };
        if (index < maxIndex)
            moveDownMenuItem.Click += (_, _) => MoveStep(control, index + 1);
        else
            moveDownMenuItem.IsEnabled = false;

        menuItems.Add(moveDownMenuItem);

        control.ContextMenu = new();
        control.ContextMenu.Items.AddRange(menuItems);
        control.ContextMenu.IsOpen = true;
    }

    private void MoveStep(UIElement control, int index)
    {
        _stepsStackPanel.Children.Remove(control);
        _stepsStackPanel.Children.Insert(index, control);

        OnChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddStep(AbstractAutomationStepControl control)
    {
        _stepsStackPanel.Children.Add(control);
        _cardHeaderControl.Subtitle = GenerateSubtitle();
        _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;

        control.Focus();

        OnChanged?.Invoke(this, EventArgs.Empty);
    }

    private void DeleteStep(UIElement control)
    {
        _stepsStackPanel.Children.Remove(control);
        _cardHeaderControl.Subtitle = GenerateSubtitle();
        _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;

        OnChanged?.Invoke(this, EventArgs.Empty);
    }
}
