using System;
using Humanizer;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class UserInactivityPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<IUserInactivityPipelineTrigger>
{
    private static readonly TimeSpan[] _timeSpans =
    {
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(3),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromMinutes(15),
        TimeSpan.FromMinutes(30)
    };

    public UserInactivityPipelineTriggerTabItemContent(IUserInactivityPipelineTrigger trigger)
    {
        InitializeComponent();

        _timeoutComboBox.SetItems(_timeSpans, trigger.InactivityTimeSpan, t => t.Humanize());
    }

    public IUserInactivityPipelineTrigger GetTrigger()
    {
        var t = _timeoutComboBox.TryGetSelectedItem(out TimeSpan tt) ? tt : TimeSpan.FromSeconds(30);
        return new UserInactivityAutomationPipelineTrigger(t);
    }
}
