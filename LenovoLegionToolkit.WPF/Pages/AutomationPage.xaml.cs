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
                _enableAutomation.IsEnabled = isChecked.Value;
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

        private async Task RefreshAsync()
        {
            var pipelines = await _automationProcessor.GetPipelinesAsync();

            _pipelinesStackPanel.Children.Clear();

            foreach (var pipeline in pipelines)
            {
                var control = GenerateControl(pipeline);
                _pipelinesStackPanel.Children.Add(control);
            }

            RefreshNewPipelineButton();
        }

        private UIElement GenerateControl(AutomationPipeline pipeline)
        {
            var control = new AutomationPipelineControl(pipeline);
            control.OnDelete += (s, e) =>
            {
                if (s is AutomationPipelineControl control)
                    DeletePipeline(control);
            };
            return control;
        }

        private void AddPipeline(AutomationPipelineTrigger trigger)
        {
            var pipeline = new AutomationPipeline(trigger);
            var control = GenerateControl(pipeline);
            _pipelinesStackPanel.Children.Add(control);

            RefreshNewPipelineButton();
        }

        private void DeletePipeline(AutomationPipelineControl control)
        {
            _pipelinesStackPanel.Children.Remove(control);

            RefreshNewPipelineButton();
        }

        private void RefreshNewPipelineButton()
        {
            var allTriggers = new[] { AutomationPipelineTrigger.ACAdapterConnected, AutomationPipelineTrigger.ACAdapterDisconnected };

            var triggers = Lib.Extensions.ListExtensions.ToArray(_pipelinesStackPanel.Children
)
                .OfType<AutomationPipelineControl>()
                .Select(c => c.AutomationPipeline)
                .SelectMany(p => p.Triggers);

            var menuItems = new List<MenuItem>();

            foreach (var trigger in allTriggers)
            {
                if (triggers.Contains(trigger))
                    continue;

                var menuItem = new MenuItem
                {
                    Icon = SymbolRegular.Flow20,
                    Header = trigger.GetDisplayName(),
                };
                menuItem.Click += (s, e) => AddPipeline(trigger);
                menuItems.Add(menuItem);
            }

            _newPipelineButton.ContextMenu.Items.Clear();
            _newPipelineButton.ContextMenu.Items.AddRange(menuItems);
            _newPipelineButton.IsEnabled = menuItems.Any();
        }
    }
}
