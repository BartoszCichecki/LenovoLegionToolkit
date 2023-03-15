using System;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class TimeTabItemContent : IAutomationPipelineTriggerTabItemContent<ITimeAutomationPipelineTrigger>
{
    private readonly bool _isSunrise;
    private readonly bool _isSunset;
    private readonly Time? _time;

    public TimeTabItemContent(ITimeAutomationPipelineTrigger trigger)
    {
        _isSunrise = trigger.IsSunrise;
        _isSunset = trigger.IsSunset;
        _time = trigger.Time;

        InitializeComponent();
    }

    private void TimeTabItem_Initialized(object? sender, EventArgs e)
    {
        _sunriseRadioButton.IsChecked = _isSunrise;
        _sunsetRadioButton.IsChecked = _isSunset;

        var local = _time is not null
            ? DateTimeExtensions.UtcFrom(_time.Value.Hour, _time.Value.Minute).ToLocalTime()
            : DateTime.Now;

        _timePickerHours.Value = local.Hour;
        _timePickerMinutes.Value = local.Minute;

        _timeRadioButton.IsChecked = _time is not null;
        _timePickerPanel.IsEnabled = _time is not null;
    }

    public ITimeAutomationPipelineTrigger GetTrigger()
    {
        var isSunrise = _sunriseRadioButton.IsChecked ?? false;
        var isSunset = _sunsetRadioButton.IsChecked ?? false;
        var utc = DateTimeExtensions.LocalFrom((int)_timePickerHours.Value, (int)_timePickerMinutes.Value).ToUniversalTime();
        Time? time = _timePickerPanel.IsEnabled ? new()
        {
            Hour = utc.Hour,
            Minute = utc.Minute,
        } : null;
        return new TimeAutomationPipelineTrigger(isSunrise, isSunset, time);
    }

    private void RadioButton_Click(object sender, RoutedEventArgs e)
    {
        _timePickerPanel.IsEnabled = _timeRadioButton.IsChecked ?? false;
    }
}
