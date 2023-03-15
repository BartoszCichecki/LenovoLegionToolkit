using System;
using System.Linq;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class PowerModeTabItemContent : IAutomationPipelineTriggerTabItemContent<IPowerModeAutomationPipelineTrigger>
{
    private readonly PowerModeFeature _feature = IoCContainer.Resolve<PowerModeFeature>();

    private readonly PowerModeState _powerModeState;

    public PowerModeTabItemContent(IPowerModeAutomationPipelineTrigger trigger)
    {
        _powerModeState = trigger.PowerModeState;

        InitializeComponent();
    }

    public IPowerModeAutomationPipelineTrigger GetTrigger()
    {
        var state = _content.Children
            .OfType<RadioButton>()
            .Where(r => r.IsChecked ?? false)
            .Select(r => (PowerModeState)r.Tag)
            .DefaultIfEmpty(PowerModeState.Balance)
            .FirstOrDefault();
        return new PowerModeAutomationPipelineTrigger(state);
    }

    private async void PowerModeTabItem_Initialized(object? sender, EventArgs eventArgs)
    {
        var states = await _feature.GetAllStatesAsync();

        foreach (var state in states)
        {
            var radio = new RadioButton
            {
                Content = state.GetDisplayName(),
                Tag = state,
                IsChecked = state == _powerModeState,
                Margin = new(0, 0, 0, 8)
            };
            _content.Children.Add(radio);
        }
    }
}