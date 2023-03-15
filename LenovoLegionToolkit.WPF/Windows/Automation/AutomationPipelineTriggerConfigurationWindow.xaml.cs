using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

namespace LenovoLegionToolkit.WPF.Windows.Automation;

public partial class AutomationPipelineTriggerConfigurationWindow
{
    private readonly List<IAutomationPipelineTrigger> _otherTriggers = new();
    private readonly IEnumerable<IAutomationPipelineTrigger> _triggers;

    public event EventHandler<IAutomationPipelineTrigger>? OnSave;

    public AutomationPipelineTriggerConfigurationWindow(IEnumerable<IAutomationPipelineTrigger> triggers)
    {
        _triggers = triggers;

        InitializeComponent();
    }

    private void AutomationPipelineTriggerConfigurationWindow_Initialized(object? sender, EventArgs e)
    {
        foreach (var trigger in _triggers)
        {
            var item = Create(trigger);
            if (item.HasValue)
            {
                _tabControl.Items.Add(new TabItem
                {
                    Header = item.Value.header,
                    Content = item.Value.content
                });
            }
            else
            {
                _otherTriggers.Add(trigger);
            }
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var triggers = _tabControl.Items
            .OfType<TabItem>()
            .Select(c => c.Content)
            .OfType<IAutomationPipelineTriggerTabItemContent<IAutomationPipelineTrigger>>()
            .Select(c => c.GetTrigger())
            .Union(_otherTriggers)
            .ToArray();

        var result = triggers.Length > 1
            ? new AndAutomationPipelineTrigger(triggers)
            : triggers.FirstOrDefault();

        if (result is not null)
            OnSave?.Invoke(this, result);

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public static bool IsValid(IEnumerable<IAutomationPipelineTrigger> triggers) => triggers.Where(IsValid).Any();

    private static bool IsValid(IAutomationPipelineTrigger trigger) => trigger switch
    {
        IPowerModeAutomationPipelineTrigger => true,
        ITimeAutomationPipelineTrigger => true,
        _ => false
    };

    private static (string header, IAutomationPipelineTriggerTabItemContent<IAutomationPipelineTrigger> content)? Create(IAutomationPipelineTrigger trigger) => trigger switch
    {
        IPowerModeAutomationPipelineTrigger pmt => ("Power Mode", new PowerModeTabItemContent(pmt)),
        ITimeAutomationPipelineTrigger tt => ("Time", new TimeTabItemContent(tt)),
        _ => null
    };
}