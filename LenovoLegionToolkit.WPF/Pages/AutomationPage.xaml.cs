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
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Controls.Automation.Pipeline;
using LenovoLegionToolkit.WPF.Utils;
using Wpf.Ui.Common;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class AutomationPage : Page
    {
        private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();

        public AutomationPage()
        {
            Initialized += AutomationPage_Initialized;

            InitializeComponent();
        }

        private async void AutomationPage_Initialized(object? sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private void EnableAutomaticPipelinesToggle_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = _enableAutomaticPipelinesToggle.IsChecked;
            if (isChecked.HasValue)
                _automationProcessor.IsEnabled = isChecked.Value;
        }

        private void NewAutomaticPipelineButton_Click(object sender, RoutedEventArgs e)
        {
            _newAutomaticPipelineButton.ContextMenu.PlacementTarget = _newAutomaticPipelineButton;
            _newAutomaticPipelineButton.ContextMenu.Placement = PlacementMode.Bottom;
            _newAutomaticPipelineButton.ContextMenu.IsOpen = true;
        }

        private async void NewManualPipelineButton_Click(object sender, RoutedEventArgs e)
        {
            await AddManualPipelineAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _saveButton.IsEnabled = false;
                _saveButton.Content = "Saving...";

                var automaticPipelines = _automaticPipelinesStackPanel.Children.ToArray()
                    .OfType<AutomationPipelineControl>()
                    .Select(c => c.CreateAutomationPipeline())
                    .ToList();

                var manualPipelines = _manualPipelinesStackPanel.Children.ToArray()
                    .OfType<AutomationPipelineControl>()
                    .Select(c => c.CreateAutomationPipeline())
                    .ToList();

                var pipelines = new List<AutomationPipeline>();
                pipelines.AddRange(automaticPipelines);
                pipelines.AddRange(manualPipelines);

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
            _scrollViewer.ScrollToTop();

            _loaderAutomatic.IsLoading = true;
            _loaderManual.IsLoading = true;

            var loadingTask = Task.Delay(500);

            var pipelines = await _automationProcessor.GetPipelinesAsync();

            _enableAutomaticPipelinesToggle.IsChecked = _automationProcessor.IsEnabled;

            _automaticPipelinesStackPanel.Children.Clear();
            _manualPipelinesStackPanel.Children.Clear();

            foreach (var pipeline in pipelines.Where(p => p.Trigger is not null))
            {
                var control = GenerateControl(pipeline, _automaticPipelinesStackPanel);
                _automaticPipelinesStackPanel.Children.Add(control);
            }

            foreach (var pipeline in pipelines.Where(p => p.Trigger is null))
            {
                var control = GenerateControl(pipeline, _manualPipelinesStackPanel);
                _manualPipelinesStackPanel.Children.Add(control);
            }

            RefreshNewAutomaticPipelineButton();

            _saveRevertStackPanel.Visibility = Visibility.Collapsed;

            _noAutomaticActionsText.Visibility = _automaticPipelinesStackPanel.Children.Count < 1
                ? Visibility.Visible
                : Visibility.Collapsed;
            _noManualActionsText.Visibility = _manualPipelinesStackPanel.Children.Count < 1
                ? Visibility.Visible
                : Visibility.Collapsed;

            await loadingTask;

            _loaderAutomatic.IsLoading = false;
            _loaderManual.IsLoading = false;
        }

        private UIElement GenerateControl(AutomationPipeline pipeline, StackPanel stackPanel)
        {
            var control = new AutomationPipelineControl(pipeline);
            control.MouseRightButtonUp += (s, e) =>
            {
                ShowPipelineContextMenu(control, stackPanel);
                e.Handled = true;
            };
            control.OnChanged += (s, e) => PipelinesChanged();
            control.OnDelete += (s, e) =>
            {
                if (s is AutomationPipelineControl control)
                    DeletePipeline(control, stackPanel);
            };
            return control;
        }

        private void PipelinesChanged()
        {
            _saveRevertStackPanel.Visibility = Visibility.Visible;
        }

        private void ShowPipelineContextMenu(AutomationPipelineControl control, StackPanel stackPanel)
        {
            var menuItems = new List<MenuItem>();

            var index = stackPanel.Children.IndexOf(control);
            var maxIndex = stackPanel.Children.Count - 1;

            var moveUpMenuItem = new MenuItem { SymbolIcon = SymbolRegular.ArrowUp24, Header = "Move up" };
            if (index > 0)
                moveUpMenuItem.Click += (s, e) => MovePipeline(control, stackPanel, index - 1);
            else
                moveUpMenuItem.IsEnabled = false;
            menuItems.Add(moveUpMenuItem);

            var moveDownMenuItem = new MenuItem { SymbolIcon = SymbolRegular.ArrowDown24, Header = "Move down" };
            if (index < maxIndex)
                moveDownMenuItem.Click += (s, e) => MovePipeline(control, stackPanel, index + 1);
            else
                moveDownMenuItem.IsEnabled = false;
            menuItems.Add(moveDownMenuItem);

            var renameMenuItem = new MenuItem { SymbolIcon = SymbolRegular.Edit24, Header = "Rename" };
            renameMenuItem.Click += async (s, e) => await RenamePipelineAsync(control);
            menuItems.Add(renameMenuItem);

            control.ContextMenu = new();
            control.ContextMenu.Items.AddRange(menuItems);
            control.ContextMenu.IsOpen = true;
        }

        private void MovePipeline(AutomationPipelineControl control, StackPanel stackPanel, int index)
        {
            stackPanel.Children.Remove(control);
            stackPanel.Children.Insert(index, control);

            PipelinesChanged();
        }

        private void AddAutomaticPipeline(IAutomationPipelineTrigger trigger)
        {
            var pipeline = new AutomationPipeline(trigger);
            var control = GenerateControl(pipeline, _automaticPipelinesStackPanel);
            _automaticPipelinesStackPanel.Children.Insert(0, control);

            RefreshNewAutomaticPipelineButton();
            PipelinesChanged();
        }

        private async Task AddManualPipelineAsync()
        {
            var newName = await MessageBoxHelper.ShowInputAsync(this, "Add new", "Name...");
            if (string.IsNullOrWhiteSpace(newName))
                return;

            var pipeline = new AutomationPipeline(newName);
            var control = GenerateControl(pipeline, _manualPipelinesStackPanel);
            _manualPipelinesStackPanel.Children.Insert(0, control);

            RefreshNewAutomaticPipelineButton();
            PipelinesChanged();
        }

        private async Task RenamePipelineAsync(AutomationPipelineControl control)
        {
            var name = control.GetName();
            var newName = await MessageBoxHelper.ShowInputAsync(this, "Rename", "Name...", name, allowEmpty: true);
            control.SetName(newName);
        }

        private void DeletePipeline(AutomationPipelineControl control, StackPanel stackPanel)
        {
            stackPanel.Children.Remove(control);

            RefreshNewAutomaticPipelineButton();
            PipelinesChanged();
        }

        private void RefreshNewAutomaticPipelineButton()
        {
            var triggers = new IAutomationPipelineTrigger[] {
                new ACAdapterConnectedAutomationPipelineTrigger(),
                new ACAdapterDisconnectedAutomationPipelineTrigger(),
                new ProcessesAreRunningAutomationPipelineTrigger(Array.Empty<ProcessInfo>()),
                new ProcessesStopRunningAutomationPipelineTrigger(Array.Empty<ProcessInfo>()),
            };

            var menuItems = new List<MenuItem>();

            foreach (var trigger in triggers)
            {
                var menuItem = new MenuItem
                {
                    Header = trigger.DisplayName,
                };

                if (AllowDuplicates(trigger))
                    menuItem.Click += (s, e) => AddAutomaticPipeline(trigger);
                else
                    menuItem.IsEnabled = false;

                menuItems.Add(menuItem);
            }

            _newAutomaticPipelineButton.ContextMenu.Items.Clear();
            _newAutomaticPipelineButton.ContextMenu.Items.AddRange(menuItems);
        }

        private bool AllowDuplicates(IAutomationPipelineTrigger trigger)
        {
            if (trigger is IDisallowDuplicatesAutomationPipelineTrigger)
            {
                var alreadyContains = _automaticPipelinesStackPanel.Children.ToArray()
                    .OfType<AutomationPipelineControl>()
                    .Select(c => c.AutomationPipeline.Trigger)
                    .Where(t => t is not null)
                    .Any(t => t!.GetType() == trigger.GetType());

                return !alreadyContains;
            }

            return true;
        }
    }
}
