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
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Controls.Automation.Steps;
using LenovoLegionToolkit.WPF.Utils;
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

        private readonly StackPanel _buttonsStackPanel = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new(0, 16, 0, 0),
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
            Content = "Delete flow",
            Appearance = Appearance.Secondary,
            Width = 100,
        };

        public AutomationPipeline AutomationPipeline { get; }

        public event EventHandler? OnChanged;
        public event EventHandler? OnDelete;

        public AutomationPipelineControl(AutomationPipeline automationPipeline)
        {
            AutomationPipeline = automationPipeline;

            InitializeComponent();
        }

        public AutomationPipeline CreateAutomationPipeline() => new()
        {
            Name = AutomationPipeline.Name,
            Triggers = AutomationPipeline.Triggers.ToList(),
            Steps = _stepsStackPanel.Children.ToArray()
                .OfType<AbstractAutomationStepControl>()
                .Select(s => s.CreateAutomationStep())
                .ToList(),
        };

        public string? GetName() => AutomationPipeline.Name;

        public void SetName(string name)
        {
            AutomationPipeline.Name = name;
            _cardExpander.Header = GenerateHeader();

            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeComponent()
        {
            foreach (var step in AutomationPipeline.Steps)
            {
                var control = GenerateControl(step);
                _stepsStackPanel.Children.Add(control);
            }

            _runNowButton.Click += async (s, e) => await RunAsync();

            _addStepButton.ContextMenu = GenerateAddStepContextMenu();
            _addStepButton.Click += (s, e) => _addStepButton.ContextMenu.IsOpen = true;

            _deletePipelineButton.Click += (s, e) => OnDelete?.Invoke(this, EventArgs.Empty);

            _buttonsStackPanel.Children.Add(_runNowButton);
            _buttonsStackPanel.Children.Add(_addStepButton);
            _buttonsStackPanel.Children.Add(_deletePipelineButton);

            _stackPanel.Children.Add(_stepsStackPanel);
            _stackPanel.Children.Add(_buttonsStackPanel);

            _cardExpander.Icon = GenerateIcon();
            _cardExpander.Header = GenerateHeader();
            _cardExpander.Subtitle = GenerateSubtitle();
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

                await SnackbarHelper.ShowAsync("Success", "Flow ran successfully!");
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
            return AutomationPipeline.Triggers.Any() ? SymbolRegular.Flow20 : SymbolRegular.DesktopFlow20;
        }

        private string GenerateHeader()
        {
            var parts = AutomationPipeline.Triggers
                .Select(t => t.GetDisplayName())
                .Where(s => !string.IsNullOrWhiteSpace(s));

            if (parts.Any())
                return "When " + string.Join(", ", parts);

            return AutomationPipeline.Name ?? "Unnamed flow";
        }

        private string GenerateSubtitle()
        {
            var stepsCount = _stepsStackPanel.Children.ToArray()
                .OfType<AbstractAutomationStepControl>()
                .Count();

            var text = $"{stepsCount} step";

            if (stepsCount != 1)
                text += "s";

            return text;
        }

        private AbstractAutomationStepControl GenerateControl(IAutomationStep step)
        {
            AbstractAutomationStepControl control = step switch
            {
                DeactivateGPUAutomationStep s => new DeactivateGPUAutomationStepControl(s),
                OverDriveAutomationStep s => new OverDriveAutomationStepControl(s),
                PowerModeAutomationStep s => new PowerModeAutomationStepControl(s),
                RefreshRateAutomationStep s => new RefreshRateAutomationStepControl(s),
                RunAutomationStep s => new RunAutomationStepControl(s),
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
            control.Delete += (s, e) =>
            {
                if (s is AbstractAutomationStepControl step)
                    DeleteStep(step);
            };
            return control;
        }

        private ContextMenu GenerateAddStepContextMenu()
        {
            var menuItems = GenerateMenuItems(new IAutomationStep[] {
                new PowerModeAutomationStep(default),
                new RefreshRateAutomationStep(default),
                new OverDriveAutomationStep(default),
                new DeactivateGPUAutomationStep(),
                new RunAutomationStep(default, default),
            });

            var contextMenu = new ContextMenu
            {
                PlacementTarget = _addStepButton,
                Placement = PlacementMode.Bottom,
            };
            contextMenu.Items.AddRange(menuItems);
            return contextMenu;
        }

        private IEnumerable<MenuItem> GenerateMenuItems(params IAutomationStep[] steps)
        {
            foreach (var step in steps)
            {
                var control = GenerateControl(step);
                var menuItem = new MenuItem { Icon = control.Icon, Header = control.Title };
                menuItem.Click += (s, e) => AddStep(control);
                yield return menuItem;
            }
        }

        private void ShowContextMenu(Control control)
        {
            var menuItems = new List<MenuItem>();

            var index = _stepsStackPanel.Children.IndexOf(control);
            var maxIndex = _stepsStackPanel.Children.Count - 1;

            var moveUpMenuItem = new MenuItem
            {
                Icon = SymbolRegular.ArrowUp24,
                Header = "Move step up"
            };
            if (index > 0)
                moveUpMenuItem.Click += (s, e) => MoveStep(control, index - 1);
            else
                moveUpMenuItem.IsEnabled = false;
            menuItems.Add(moveUpMenuItem);

            var moveDownMenuItem = new MenuItem
            {
                Icon = SymbolRegular.ArrowDown24,
                Header = "Move step down"
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

        private void AddStep(AbstractAutomationStepControl control)
        {
            _stepsStackPanel.Children.Add(control);
            _cardExpander.Subtitle = GenerateSubtitle();

            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteStep(Control control)
        {
            _stepsStackPanel.Children.Remove(control);
            _cardExpander.Subtitle = GenerateSubtitle();

            OnChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
