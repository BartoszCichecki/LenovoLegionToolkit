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
using LenovoLegionToolkit.WPF.Controls.Automation.Steps;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Automation;
using WPFUI.Common;
using WPFUI.Controls;
using Button = WPFUI.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Pipeline
{
    public class AutomationPipelineControl : UserControl
    {
        private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();

        private readonly CardExpander _cardExpander = new()
        {
            Margin = new(0, 0, 0, 8),
        };

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
            Content = "Exclusive",
            ToolTip = "Do not execute further actions when this action runs.",
            Width = 100,
            Margin = new(0, 0, 8, 0),
        };

        private readonly Button _runNowButton = new()
        {
            Content = "Run now",
            Appearance = Appearance.Secondary,
            Width = 100,
            Margin = new(0, 0, 8, 0),
        };

        private readonly Button _addStepButton = new()
        {
            Content = "Add step",
            Appearance = Appearance.Secondary,
            Width = 100,
            Margin = new(0, 0, 8, 0),
        };

        private readonly Button _deletePipelineButton = new()
        {
            Content = "Delete",
            Appearance = Appearance.Secondary,
            Width = 100,
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
            _cardExpander.Header = GenerateHeader();
            _cardExpander.Subtitle = GenerateSubtitle();

            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void AutomationPipelineControl_Initialized(object? sender, EventArgs e)
        {
            _cardExpander.Icon = GenerateIcon();
            _cardExpander.Header = GenerateHeader();
            _cardExpander.Subtitle = GenerateSubtitle();
            _cardExpander.HeaderContent = GenerateAccessory();

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
            _cardExpander.Content = _stackPanel;

            Content = _cardExpander;
        }

        private async Task RunAsync()
        {
            try
            {
                _runNowButton.IsEnabled = false;
                _runNowButton.Content = "Running...";
                var pipeline = CreateAutomationPipeline();
                await _automationProcessor.RunNowAsync(pipeline);

                await SnackbarHelper.ShowAsync("Success", "Action ran successfully!");
            }
            catch (Exception ex)
            {
                await SnackbarHelper.ShowAsync("Run failed", ex.Message);
            }
            finally
            {
                _runNowButton.Content = "Run now";
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

            return "Unnamed";
        }

        private string GenerateSubtitle()
        {
            var stepsCount = _stepsStackPanel.Children.ToArray()
                .OfType<AbstractAutomationStepControl>()
                .Count();
            var stepsCountModifier = stepsCount != 1 ? "s" : "";

            var result = $"{stepsCount} step{stepsCountModifier}";

            if (!string.IsNullOrWhiteSpace(AutomationPipeline.Name) && AutomationPipeline.Trigger is not null)
                result += $" | {AutomationPipeline.Trigger.DisplayName}";

            return result;
        }

        private object? GenerateAccessory()
        {
            if (AutomationPipeline.Trigger is IProcessesAutomationPipelineTrigger t)
            {
                var button = new Button
                {
                    Appearance = Appearance.Secondary,
                    Content = "Configure",
                    Margin = new(0, 0, 16, 0),
                    Width = 120,
                };
                button.Click += (s, e) =>
                {
                    var window = new PickProcessesWindow(t.Processes)
                    {
                        Owner = Window.GetWindow(this),
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        ShowInTaskbar = false,
                    };
                    window.OnSave += (s, e) =>
                    {
                        AutomationPipeline.Trigger = t.DeepCopy(e);
                        _cardExpander.HeaderContent = GenerateAccessory();
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
                new DeactivateGPUAutomationStep(),
                new DelayAutomationStep(default),
                new FlipToStartAutomationStep(default),
                new FnLockAutomationStep(default),
                new OverDriveAutomationStep(default),
                new PowerModeAutomationStep(default),
                new RefreshRateAutomationStep(default),
                new RunAutomationStep(default, default),
                new TouchpadLockAutomationStep(default),
            };

            var menuItems = new List<MenuItem>();

            foreach (var step in steps)
            {
                if (!await step.IsSupportedAsync())
                    continue;

                var control = GenerateStepControl(step);
                var menuItem = new MenuItem { Icon = control.Icon, Header = control.Title };
                if (AllowDuplicates(step))
                    menuItem.Click += async (s, e) => await AddStepAsync(control);
                else
                    menuItem.IsEnabled = false;
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
                FlipToStartAutomationStep s => new FlipToStartAutomationStepControl(s),
                FnLockAutomationStep s => new FnLockAutomationStepControl(s),
                OverDriveAutomationStep s => new OverDriveAutomationStepControl(s),
                PowerModeAutomationStep s => new PowerModeAutomationStepControl(s),
                RefreshRateAutomationStep s => new RefreshRateAutomationStepControl(s),
                RunAutomationStep s => new RunAutomationStepControl(s),
                TouchpadLockAutomationStep s => new TouchpadLockAutomationStepControl(s),
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
                Icon = SymbolRegular.ArrowUp24,
                Header = "Move up"
            };
            if (index > 0)
                moveUpMenuItem.Click += (s, e) => MoveStep(control, index - 1);
            else
                moveUpMenuItem.IsEnabled = false;
            menuItems.Add(moveUpMenuItem);

            var moveDownMenuItem = new MenuItem
            {
                Icon = SymbolRegular.ArrowDown24,
                Header = "Move down"
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
            if (!AllowDuplicates(control.AutomationStep))
                return;

            _stepsStackPanel.Children.Add(control);
            _cardExpander.Subtitle = GenerateSubtitle();
            _addStepButton.ContextMenu = await CreateAddStepContextMenuAsync();

            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        private async Task DeleteStepAsync(Control control)
        {
            _stepsStackPanel.Children.Remove(control);
            _cardExpander.Subtitle = GenerateSubtitle();
            _addStepButton.ContextMenu = await CreateAddStepContextMenuAsync();

            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool AllowDuplicates(IAutomationStep step)
        {
            if (step is IDisallowDuplicatesAutomationStep)
            {
                var alreadyContains = _stepsStackPanel.Children.ToArray()
                    .OfType<AbstractAutomationStepControl>()
                    .Select(c => c.AutomationStep)
                    .Any(s => s.GetType() == step.GetType());

                return !alreadyContains;
            }

            return true;
        }
    }
}
