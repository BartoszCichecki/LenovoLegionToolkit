using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Automation;

public partial class AutomationPipelineTriggerConfigurationWindow
{
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
                _tabControl.Items.Add(new TabItem
                {
                    Visibility = Visibility.Collapsed,
                    Tag = trigger
                });
            }
        }

        if (_tabControl.Items.Count < 2)
            return;

        _tabControl.SelectedIndex = (_tabControl.Items[0] as TabItem)?.Content is null ? 1 : 0;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var triggers = _tabControl.Items
            .OfType<TabItem>()
            .Select(c =>
            {
                if (c.Content is IAutomationPipelineTriggerTabItemContent<IAutomationPipelineTrigger> content)
                    return content.GetTrigger();

                if (c.Tag is IAutomationPipelineTrigger trigger)
                    return trigger;

                return null;
            })
            .OfType<IAutomationPipelineTrigger>()
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
