using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.WPF.Controls.Automation.Pipeline;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class AutomationPage : Page
    {
        private readonly AutomationProcessor _automationProcessor = DIContainer.Resolve<AutomationProcessor>();

        private List<AutomationPipeline>? _pipelines;

        public AutomationPage()
        {
            InitializeComponent();

            Loaded += AutomationPage_Loaded;
        }

        private async void AutomationPage_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private void EnableAutomation_Click(object sender, RoutedEventArgs e)
        {

        }

        private async Task RefreshAsync()
        {
            if (_pipelines is not null)
                return;

            _pipelines = await _automationProcessor.GetPipelinesAsync();

            _pipelinesStackPanel.Children.Clear();

            foreach (var pipeline in _pipelines)
            {
                var control = new AutomationPipelineControl(pipeline);
                control.OnDelete += (s, e) =>
                {
                    if (s is AutomationPipelineControl control)
                        DeletePipeline(control);
                };
                _pipelinesStackPanel.Children.Add(control);
            }
        }

        private void DeletePipeline(AutomationPipelineControl control)
        {
            _pipelines?.Remove(control.AutomationPipeline);
            _pipelinesStackPanel.Children.Remove(control);
        }
    }
}
