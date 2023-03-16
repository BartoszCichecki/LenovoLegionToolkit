using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;
using Wpf.Ui.Controls;

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
            var content = Create(trigger);
            if (content is not null)
            {
                var header = new StackPanel { Orientation = Orientation.Horizontal };
                header.Children.Add(new SymbolIcon { Symbol = trigger.Icon(), Margin = new(8, 0, 0, 0) });
                header.Children.Add(new TextBlock { Text = trigger.DisplayName, Margin = new(4, 0, 8, 0) });

                _tabControl.Items.Add(new TabItem
                {
                    Header = header,
                    Content = content
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

    public static bool IsValid(IEnumerable<IAutomationPipelineTrigger> triggers) => triggers.Any(IsValid);

    private static bool IsValid(IAutomationPipelineTrigger trigger) => trigger switch
    {
        IPowerModeAutomationPipelineTrigger => true,
        ITimeAutomationPipelineTrigger => true,
        _ => false
    };

    private static IAutomationPipelineTriggerTabItemContent<IAutomationPipelineTrigger>? Create(IAutomationPipelineTrigger trigger) => trigger switch
    {
        IPowerModeAutomationPipelineTrigger pmt => new PowerModeAutomationPipelineTriggerTabItemContent(pmt),
        ITimeAutomationPipelineTrigger tt => new TimeAutomationPipelineTriggerTabItemContent(tt),
        _ => null
    };
}
