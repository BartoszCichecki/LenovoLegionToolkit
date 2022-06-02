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
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Controls.Automation.Pipeline;
using LenovoLegionToolkit.WPF.Utils;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class AutomationPage : Page
    {
        private readonly AutomationProcessor _automationProcessor = DIContainer.Resolve<AutomationProcessor>();

        public AutomationPage()
        {
            InitializeComponent();

            Loaded += AutomationPage_Loaded;
        }

        private async void AutomationPage_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private void EnableAutomation_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = _enableAutomation.IsChecked;
            if (isChecked.HasValue)
                _automationProcessor.IsEnabled = isChecked.Value;
        }

        private void NewPipelineButton_Click(object sender, RoutedEventArgs e)
        {
            _newPipelineButton.ContextMenu.PlacementTarget = _newPipelineButton;
            _newPipelineButton.ContextMenu.Placement = PlacementMode.Bottom;
            _newPipelineButton.ContextMenu.IsOpen = true;
        }

        private void NewPipelineButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _newPipelineButton.Appearance = _newPipelineButton.IsEnabled
                ? Appearance.Primary
                : Appearance.Transparent;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _saveButton.IsEnabled = false;
                _saveButton.Content = "Saving...";

                var pipelines = _pipelinesStackPanel.Children.ToArray()
                    .OfType<AutomationPipelineControl>()
                    .Select(c => c.CreateAutomationPipeline())
                    .ToList();

                await _automationProcessor.ReloadPipelinesAsync(pipelines);
                await RefreshAsync();

                await SnackbarHelper.ShowAsync("Saved", "Changes were saved successfully!");
            }
            finally
            {
                _saveButton.Content = "Save";
                _saveButton.IsEnabled = true;
            }
        }

        private async void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshAsync();

            await SnackbarHelper.ShowAsync("Reverted", "All changes reverted!");
        }

        private async Task RefreshAsync()
        {
            var pipelines = await _automationProcessor.GetPipelinesAsync();

            _enableAutomation.IsChecked = _automationProcessor.IsEnabled;

            _pipelinesStackPanel.Children.Clear();

            foreach (var pipeline in pipelines)
            {
                var control = GenerateControl(pipeline);
                _pipelinesStackPanel.Children.Add(control);
            }

            RefreshNewPipelineButton();

            _saveRevertStackPanel.Visibility = Visibility.Collapsed;
        }

        private UIElement GenerateControl(AutomationPipeline pipeline)
        {
            var control = new AutomationPipelineControl(pipeline);
            control.MouseRightButtonUp += (s, e) =>
            {
                ShowContextMenu(control);
                e.Handled = true;
            };
            control.OnChanged += (s, e) => PipelinesChanged();
            control.OnDelete += (s, e) =>
            {
                if (s is AutomationPipelineControl control)
                    DeletePipeline(control);
            };
            return control;
        }

        private void PipelinesChanged()
        {
            _saveRevertStackPanel.Visibility = Visibility.Visible;
        }

        private void ShowContextMenu(AutomationPipelineControl control)
        {
            var menuItems = new List<MenuItem>();

            var index = _pipelinesStackPanel.Children.IndexOf(control);
            var maxIndex = _pipelinesStackPanel.Children.Count - 1;

            if (index > 0)
            {
                var menuItem = new MenuItem { Icon = SymbolRegular.ArrowUp24, Header = "Move flow up" };
                menuItem.Click += (s, e) => MovePipeline(control, index - 1);
                menuItems.Add(menuItem);
            }

            if (index < maxIndex)
            {
                var menuItem = new MenuItem { Icon = SymbolRegular.ArrowDown24, Header = "Move flow down" };
                menuItem.Click += (s, e) => MovePipeline(control, index + 1);
                menuItems.Add(menuItem);
            }

            if (menuItems.Count < 1)
                return;

            control.ContextMenu = new();
            control.ContextMenu.Items.AddRange(menuItems);
            control.ContextMenu.IsOpen = true;
        }

        private void MovePipeline(AutomationPipelineControl control, int index)
        {
            _pipelinesStackPanel.Children.Remove(control);
            _pipelinesStackPanel.Children.Insert(index, control);

            PipelinesChanged();
        }

        private void AddPipeline(AutomationPipelineTrigger? trigger)
        {
            AutomationPipeline pipeline;
            if (trigger is null)
                pipeline = new AutomationPipeline();
            else
                pipeline = new AutomationPipeline(trigger.Value);

            var control = GenerateControl(pipeline);
            _pipelinesStackPanel.Children.Add(control);

            RefreshNewPipelineButton();
            PipelinesChanged();
        }

        private void DeletePipeline(AutomationPipelineControl control)
        {
            _pipelinesStackPanel.Children.Remove(control);

            RefreshNewPipelineButton();
            PipelinesChanged();
        }

        private void RefreshNewPipelineButton()
        {
            var allTriggers = new[] {
                AutomationPipelineTrigger.ACAdapterConnected,
                AutomationPipelineTrigger.ACAdapterDisconnected,
            };

            var triggers = _pipelinesStackPanel.Children.ToArray()
                .OfType<AutomationPipelineControl>()
                .Select(c => c.AutomationPipeline)
                .SelectMany(p => p.Triggers);

            var menuItems = new List<MenuItem>();

            foreach (var trigger in allTriggers)
            {
                var menuItem = new MenuItem
                {
                    Icon = SymbolRegular.Flow20,
                    Header = trigger.GetDisplayName(),
                    IsEnabled = !triggers.Contains(trigger),
                };
                menuItem.Click += (s, e) => AddPipeline(trigger);
                menuItems.Add(menuItem);
            }

            var customMenuItem = new MenuItem
            {
                Icon = SymbolRegular.DesktopFlow20,
                Header = "Custom flow",
            };
            customMenuItem.Click += (s, e) => AddPipeline(null);
            menuItems.Add(customMenuItem);


            _newPipelineButton.ContextMenu.Items.Clear();
            _newPipelineButton.ContextMenu.Items.AddRange(menuItems);
        }
    }
}
