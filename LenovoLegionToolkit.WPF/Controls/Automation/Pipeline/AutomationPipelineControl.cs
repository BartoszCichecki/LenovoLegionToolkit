using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Controls.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Automation;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Pipeline;

public class AutomationPipelineControl : UserControl
{
    private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();

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

    public AutomationPipeline AutomationPipeline { get; }

    public event EventHandler? OnChanged;
    public event EventHandler? OnDelete;

    public AutomationPipelineControl(AutomationPipeline automationPipeline)
    {
        AutomationPipeline = automationPipeline;

        Initialized += AutomationPipelineControl_Initialized;
    }

    public AutomationPipeline CreateAutomationPipeline() => new()
    {
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

    private async void AutomationPipelineControl_Initialized(object? sender, EventArgs e)
    {
        _cardExpander.Header = _cardHeaderControl;

        foreach (var step in AutomationPipeline.Steps)
        {
            var control = GenerateStepControl(step);
            _stepsStackPanel.Children.Add(control);
        }

        if (AutomationPipeline.Trigger is not null)
        {
            _isExclusiveCheckBox.IsChecked = AutomationPipeline.IsExclusive;
            _isExclusiveCheckBox.Checked += (s, e) => OnChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _isExclusiveCheckBox.Visibility = Visibility.Hidden;
        }

        _runNowButton.Click += async (s, e) => await RunAsync();

        _addStepButton.ContextMenu = await CreateAddStepContextMenuAsync();
        _addStepButton.Click += (s, e) =>
        {
            if (_addStepButton.ContextMenu is not null)
                _addStepButton.ContextMenu.IsOpen = true;
        };

        _deletePipelineButton.Click += (s, e) => OnDelete?.Invoke(this, EventArgs.Empty);

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
        return AutomationPipeline.Trigger is not null ? SymbolRegular.Flow20 : SymbolRegular.Play24;
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

        if (AutomationPipeline.Trigger is IPowerModeAutomationPipelineTrigger p)
            result += $" | {Resource.AutomationPipelineControl_SubtitlePart_PowerMode}: {p.PowerModeState.GetDisplayName()}";

        if (AutomationPipeline.Trigger is IProcessesAutomationPipelineTrigger pt && pt.Processes.Any())
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
                result += $" | {string.Format(Resource.AutomationPipelineControl_SubtitlePart_AtTime, local.Hour, local.Minute)}";
            }
        }

        return result;
    }

    private UIElement? GenerateAccessory()
    {
        if (AutomationPipeline.Trigger is IPowerModeAutomationPipelineTrigger pmt)
        {
            var button = new Button
            {
                Content = Resource.AutomationPipelineControl_Configure,
                Margin = new(16, 0, 16, 0),
                MinWidth = 120,
            };
            button.Click += (s, e) =>
            {
                var window = new PowerModeWindow(pmt.PowerModeState) { Owner = Window.GetWindow(this) };
                window.OnSave += (s, e) =>
                {
                    AutomationPipeline.Trigger = pmt.DeepCopy(e);
                    _cardHeaderControl.Subtitle = GenerateSubtitle();
                    _cardHeaderControl.Accessory = GenerateAccessory();
                    _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;
                    OnChanged?.Invoke(this, EventArgs.Empty);
                };
                window.ShowDialog();
            };
            return button;
        }

        if (AutomationPipeline.Trigger is IProcessesAutomationPipelineTrigger pt)
        {
            var button = new Button
            {
                Content = Resource.AutomationPipelineControl_Configure,
                Margin = new(16, 0, 16, 0),
                MinWidth = 120,
            };
            button.Click += (s, e) =>
            {
                var window = new PickProcessesWindow(pt.Processes) { Owner = Window.GetWindow(this) };
                window.OnSave += (s, e) =>
                {
                    AutomationPipeline.Trigger = pt.DeepCopy(e);
                    _cardHeaderControl.Subtitle = GenerateSubtitle();
                    _cardHeaderControl.Accessory = GenerateAccessory();
                    _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;
                    OnChanged?.Invoke(this, EventArgs.Empty);
                };
                window.ShowDialog();
            };
            return button;
        }

        if (AutomationPipeline.Trigger is ITimeAutomationPipelineTrigger tt)
        {

            var button = new Button
            {
                Content = Resource.AutomationPipelineControl_Configure,
                Margin = new(16, 0, 16, 0),
                MinWidth = 120,
            };
            button.Click += (s, e) =>
            {
                var window = new TimeWindow(tt.IsSunrise, tt.IsSunset, tt.Time) { Owner = Window.GetWindow(this) };
                window.OnSave += (s, e) =>
                {
                    AutomationPipeline.Trigger = tt.DeepCopy(e.Item1, e.Item2, e.Item3);
                    _cardHeaderControl.Subtitle = GenerateSubtitle();
                    _cardHeaderControl.Accessory = GenerateAccessory();
                    _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;
                    OnChanged?.Invoke(this, EventArgs.Empty);
                };
                window.ShowDialog();
            };
            return button;
        }

        return null;
    }

    private async Task<ContextMenu> CreateAddStepContextMenuAsync()
    {
        var steps = new IAutomationStep[] {
            new AlwaysOnUsbAutomationStep(default),
            new BatteryAutomationStep(default),
            new DeactivateGPUAutomationStep(default),
            new DelayAutomationStep(default),
            new DisplayBrightnessAutomationStep(50),
            new DpiScaleAutomationStep(default),
            new FlipToStartAutomationStep(default),
            new FnLockAutomationStep(default),
            new HDRAutomationStep(default),
            new MicrophoneAutomationStep(default),
            new OneLevelWhiteKeyboardBacklightAutomationStep(default),
            new OverDriveAutomationStep(default),
            new PowerModeAutomationStep(default),
            new RefreshRateAutomationStep(default),
            new ResolutionAutomationStep(default),
            new RGBKeyboardBacklightAutomationStep(default),
            new RunAutomationStep(default, default),
            new SpectrumKeyboardBacklightBrightnessAutomationStep(0),
            new SpectrumKeyboardBacklightProfileAutomationStep(1),
            new TouchpadLockAutomationStep(default),
            new WhiteKeyboardBacklightAutomationStep(default),
            new WinKeyAutomationStep(default),
        };

        var menuItems = new List<MenuItem>();

        foreach (var step in steps)
        {
            if (!await step.IsSupportedAsync())
                continue;

            var control = GenerateStepControl(step);
            var menuItem = new MenuItem { SymbolIcon = control.Icon, Header = control.Title };
            menuItem.Click += async (s, e) => await AddStepAsync(control);
            menuItems.Add(menuItem);
        }

        var contextMenu = new ContextMenu
        {
            PlacementTarget = _addStepButton,
            Placement = PlacementMode.Bottom,
        };

        foreach (var menuItem in menuItems.OrderBy(mi => mi.Header))
            contextMenu.Items.Add(menuItem);

        return contextMenu;
    }

    private AbstractAutomationStepControl GenerateStepControl(IAutomationStep step)
    {
        AbstractAutomationStepControl control = step switch
        {
            AlwaysOnUsbAutomationStep s => new AlwaysOnUsbAutomationStepControl(s),
            BatteryAutomationStep s => new BatteryAutomationStepControl(s),
            DeactivateGPUAutomationStep s => new DeactivateGPUAutomationStepControl(s),
            DelayAutomationStep s => new DelayAutomationStepControl(s),
            DisplayBrightnessAutomationStep s => new DisplayBrightnessAutomationStepControl(s),
            DpiScaleAutomationStep s => new DpiScaleAutomationStepControl(s),
            FlipToStartAutomationStep s => new FlipToStartAutomationStepControl(s),
            FnLockAutomationStep s => new FnLockAutomationStepControl(s),
            HDRAutomationStep s => new HDRAutomationStepControl(s),
            MicrophoneAutomationStep s => new MicrophoneAutomationStepControl(s),
            OneLevelWhiteKeyboardBacklightAutomationStep s => new OneLevelWhiteKeyboardBacklightAutomationStepControl(s),
            OverDriveAutomationStep s => new OverDriveAutomationStepControl(s),
            PowerModeAutomationStep s => new PowerModeAutomationStepControl(s),
            RefreshRateAutomationStep s => new RefreshRateAutomationStepControl(s),
            ResolutionAutomationStep s => new ResolutionAutomationStepControl(s),
            RGBKeyboardBacklightAutomationStep s => new RGBKeyboardBacklightAutomationStepControl(s),
            RunAutomationStep s => new RunAutomationStepControl(s),
            SpectrumKeyboardBacklightBrightnessAutomationStep s => new SpectrumKeyboardBacklightBrightnessAutomationStepControl(s),
            SpectrumKeyboardBacklightProfileAutomationStep s => new SpectrumKeyboardBacklightProfileAutomationStepControl(s),
            TouchpadLockAutomationStep s => new TouchpadLockAutomationStepControl(s),
            WhiteKeyboardBacklightAutomationStep s => new WhiteKeyboardBacklightAutomationStepControl(s),
            WinKeyAutomationStep s => new WinKeyAutomationStepControl(s),
            _ => throw new InvalidOperationException("Unknown step type."),
        };
        control.MouseRightButtonUp += (s, e) =>
        {
            ShowContextMenu(control);
            e.Handled = true;
        };
        control.Changed += (s, e) =>
        {
            OnChanged?.Invoke(this, EventArgs.Empty);
        };
        control.Delete += async (s, e) =>
        {
            if (s is AbstractAutomationStepControl step)
                await DeleteStepAsync(step);
        };
        return control;
    }

    private void ShowContextMenu(Control control)
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
            moveUpMenuItem.Click += (s, e) => MoveStep(control, index - 1);
        else
            moveUpMenuItem.IsEnabled = false;
        menuItems.Add(moveUpMenuItem);

        var moveDownMenuItem = new MenuItem
        {
            SymbolIcon = SymbolRegular.ArrowDown24,
            Header = Resource.MoveDown
        };
        if (index < maxIndex)
            moveDownMenuItem.Click += (s, e) => MoveStep(control, index + 1);
        else
            moveDownMenuItem.IsEnabled = false;

        menuItems.Add(moveDownMenuItem);

        control.ContextMenu = new();
        control.ContextMenu.Items.AddRange(menuItems);
        control.ContextMenu.IsOpen = true;
    }

    private void MoveStep(Control control, int index)
    {
        _stepsStackPanel.Children.Remove(control);
        _stepsStackPanel.Children.Insert(index, control);

        OnChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task AddStepAsync(AbstractAutomationStepControl control)
    {
        _stepsStackPanel.Children.Add(control);
        _cardHeaderControl.Subtitle = GenerateSubtitle();
        _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;
        _addStepButton.ContextMenu = await CreateAddStepContextMenuAsync();

        OnChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task DeleteStepAsync(Control control)
    {
        _stepsStackPanel.Children.Remove(control);
        _cardHeaderControl.Subtitle = GenerateSubtitle();
        _cardHeaderControl.SubtitleToolTip = _cardHeaderControl.Subtitle;
        _addStepButton.ContextMenu = await CreateAddStepContextMenuAsync();

        OnChanged?.Invoke(this, EventArgs.Empty);
    }
}