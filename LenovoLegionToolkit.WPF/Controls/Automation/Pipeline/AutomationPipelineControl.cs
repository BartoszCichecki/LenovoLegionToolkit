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
        private readonly AutomationProcessor _automationProcessor = DIContainer.Resolve<AutomationProcessor>();

        private readonly CardExpander _cardExpander = new()
        {
            Icon = SymbolRegular.Flow20,
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

        public AutomationPipeline CreateAutomationPipeline()
        {
            return new()
            {
                Triggers = AutomationPipeline.Triggers,
                Steps = _stepsStackPanel.Children.ToArray().OfType<AbstractAutomationStepControl>().Select(s => s.CreateAutomationStep()).ToList(),
            };
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

            _cardExpander.Header = GenerateHeader();
            _cardExpander.Subtitle = GenerateSubtitle();
            _cardExpander.Content = _stackPanel;

            Content = _cardExpander;
        }

        private async Task RunAsync()
        {
            _runNowButton.IsEnabled = false;
            _runNowButton.Content = "Running...";
            var pipeline = CreateAutomationPipeline();
            await _automationProcessor.RunNowAsync(pipeline);
            _runNowButton.Content = "Run now";
            _runNowButton.IsEnabled = true;

            await SnackbarHelper.ShowAsync("Success", "Flow ran successfully!");
        }

        private string GenerateHeader()
        {
            var parts = AutomationPipeline.Triggers
                .Select(t => t.GetDisplayName())
                .Where(s => !string.IsNullOrWhiteSpace(s));
            return "When " + string.Join(", ", parts);
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
                ScriptAutomationStep s => new ScriptAutomationStepControl(s),
                _ => throw new InvalidOperationException("Unknown step type."),
            };
            control.MouseRightButtonUp += (s, e) =>
            {
                ShowContextMenu(control);
                e.Handled = true;
            };
            control.OnChanged += (s, e) =>
            {
                OnChanged?.Invoke(this, EventArgs.Empty);
            };
            control.OnDelete += (s, e) =>
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
                new ScriptAutomationStep(default, default),
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

            if (index > 0)
            {
                var menuItem = new MenuItem { Icon = SymbolRegular.ArrowUp24, Header = "Move step up" };
                menuItem.Click += (s, e) => MoveStep(control, index - 1);
                menuItems.Add(menuItem);
            }

            if (index < maxIndex)
            {
                var menuItem = new MenuItem { Icon = SymbolRegular.ArrowDown24, Header = "Move step down" };
                menuItem.Click += (s, e) => MoveStep(control, index + 1);
                menuItems.Add(menuItem);
            }

            if (menuItems.Count < 1)
                return;

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
