using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class QuickActionAutomationStepControl : AbstractAutomationStepControl<QuickActionAutomationStep>
{
    private readonly AutomationProcessor _processor = IoCContainer.Resolve<AutomationProcessor>();

    private readonly ComboBox _comboBox = new()
    {
        MinWidth = 150
    };

    private readonly StackPanel _stackPanel = new();

    private bool _isRefreshing;

    public QuickActionAutomationStepControl(QuickActionAutomationStep step) : base(step)
    {
        Icon = SymbolRegular.Play24;
        Title = Resource.QuickActionAutomationStepControl_Title;
        Subtitle = Resource.QuickActionAutomationStepControl_Message;
    }

    public override IAutomationStep CreateAutomationStep() => new QuickActionAutomationStep(GetSelectedPipelineIdAsync().Result);

    protected override UIElement GetCustomControl()
    {
        _comboBox.SelectionChanged += async (_, _) =>
        {
            if (_isRefreshing)
            {
                return;
            }

            var selectedPipelineId = await GetSelectedPipelineIdAsync();
            if (selectedPipelineId != AutomationStep.PipelineId)
            {
                RaiseChanged();
            }
        };

        _stackPanel.Children.Add(_comboBox);

        return _stackPanel;
    }

    protected override void OnFinishedLoading() { }

    protected override async Task RefreshAsync()
    {
        _isRefreshing = true;

        _comboBox.Items.Clear();

        var index = 0;
        var selectedIndex = -1;
        var pipelines = await _processor.GetPipelinesAsync();

        foreach (var pipeline in pipelines.Where(p => p.Trigger is null))
        {
            _comboBox.Items.Add(pipeline.Name);
            if (pipeline.Id == AutomationStep.PipelineId)
            {
                selectedIndex = index;
            }
            index++;
        }
        _comboBox.SelectedIndex = selectedIndex;

        _isRefreshing = false;
    }

    private async Task<Guid?> GetSelectedPipelineIdAsync()
    {
        var value = (string)_comboBox.SelectedItem;
        var pipelines = await _processor.GetPipelinesAsync();
        var selectedPipeline = pipelines.Where(p => p.Trigger is null).FirstOrDefault(p => p.Name == value);
        if (selectedPipeline is not null)
        {
            return selectedPipeline.Id;
        }
        return null;
    }
}
