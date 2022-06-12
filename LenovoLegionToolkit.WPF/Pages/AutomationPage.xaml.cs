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

        private void EnableAutomationToggle_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = _enableAutomationToggle.IsChecked;
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
            _scrollViewer.ScrollToTop();
            _newPipelineButton.IsEnabled = false;
            _enableAutomationToggle.Visibility = Visibility.Hidden;
            _loader.IsLoading = true;

            var loadingTask = Task.Delay(500);

            var pipelines = await _automationProcessor.GetPipelinesAsync();

            _enableAutomationToggle.IsChecked = _automationProcessor.IsEnabled;

            _pipelinesStackPanel.Children.Clear();

            foreach (var pipeline in pipelines)
            {
                var control = GenerateControl(pipeline);
                _pipelinesStackPanel.Children.Add(control);
            }

            RefreshNewPipelineButton();

            await loadingTask;

            _saveRevertStackPanel.Visibility = Visibility.Collapsed;

            _newPipelineButton.IsEnabled = true;
            _enableAutomationToggle.Visibility = Visibility.Visible;
            _loader.IsLoading = false;
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

            var moveUpMenuItem = new MenuItem { Icon = SymbolRegular.ArrowUp24, Header = "Move up" };
            if (index > 0)
                moveUpMenuItem.Click += (s, e) => MovePipeline(control, index - 1);
            else
                moveUpMenuItem.IsEnabled = false;
            menuItems.Add(moveUpMenuItem);

            var moveDownMenuItem = new MenuItem { Icon = SymbolRegular.ArrowDown24, Header = "Move down" };
            if (index < maxIndex)
                moveDownMenuItem.Click += (s, e) => MovePipeline(control, index + 1);
            else
                moveDownMenuItem.IsEnabled = false;
            menuItems.Add(moveDownMenuItem);

            var renameMenuItem = new MenuItem { Icon = SymbolRegular.Edit24, Header = "Rename" };
            renameMenuItem.Click += async (s, e) => await RenamePipelineAsync(control);
            menuItems.Add(renameMenuItem);

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

        private void AddPipeline(IAutomationPipelineTrigger trigger)
        {
            var pipeline = new AutomationPipeline(trigger);
            var control = GenerateControl(pipeline);
            _pipelinesStackPanel.Children.Insert(0, control);

            RefreshNewPipelineButton();
            PipelinesChanged();
        }

        private async Task AddPipelineAsync()
        {
            var newName = await MessageBoxHelper.ShowInputAsync(this, "Add new", "Name...");
            if (string.IsNullOrWhiteSpace(newName))
                return;

            var pipeline = new AutomationPipeline(newName);
            var control = GenerateControl(pipeline);
            _pipelinesStackPanel.Children.Insert(0, control);

            RefreshNewPipelineButton();
            PipelinesChanged();
        }

        private async Task RenamePipelineAsync(AutomationPipelineControl control)
        {
            var name = control.GetName();
            var newName = await MessageBoxHelper.ShowInputAsync(this, "Rename", "Name...", name, allowEmpty: true);
            control.SetName(newName);
        }

        private void DeletePipeline(AutomationPipelineControl control)
        {
            _pipelinesStackPanel.Children.Remove(control);

            RefreshNewPipelineButton();
            PipelinesChanged();
        }

        private void RefreshNewPipelineButton()
        {
            var allTriggers = new IAutomationPipelineTrigger[] {
                new ACAdapterConnectedAutomationPipelineTrigger(),
                new ACAdapterDisconnectedAutomationPipelineTrigger(),
            };

            var triggers = _pipelinesStackPanel.Children.ToArray()
                .OfType<AutomationPipelineControl>()
                .Select(c => c.AutomationPipeline)
                .Select(p => p.Trigger);

            var menuItems = new List<MenuItem>();

            foreach (var trigger in allTriggers)
            {
                var menuItem = new MenuItem
                {
                    Icon = SymbolRegular.Flow20,
                    Header = trigger.DisplayName,
                };
                if (triggers.Contains(trigger))
                    menuItem.IsEnabled = false;
                else
                    menuItem.Click += (s, e) => AddPipeline(trigger);
                menuItems.Add(menuItem);
            }

            var quickActionMenuItem = new MenuItem
            {
                Icon = SymbolRegular.Play24,
                Header = "Quick action",
            };
            ToolTipService.SetToolTip(quickActionMenuItem, "Quick actions appear in right click menu of the tray icon.");
            quickActionMenuItem.Click += async (s, e) => await AddPipelineAsync();
            menuItems.Add(quickActionMenuItem);


            _newPipelineButton.ContextMenu.Items.Clear();
            _newPipelineButton.ContextMenu.Items.AddRange(menuItems);
        }
    }
}
