using System;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class TimeAutomationPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<ITimeAutomationPipelineTrigger>
{
    private readonly ITimeAutomationPipelineTrigger _trigger;
    private readonly bool _isSunrise;
    private readonly bool _isSunset;
    private readonly Time? _time;
    private readonly Day? _day;

    public TimeAutomationPipelineTriggerTabItemContent(ITimeAutomationPipelineTrigger trigger)
    {
        _trigger = trigger;
        _isSunrise = trigger.IsSunrise;
        _isSunset = trigger.IsSunset;
        _time = trigger.Time;
        _day = trigger.Day;

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

        _datePickerHours.Value = local.Hour;
        _datePickerMinutes.Value = local.Minute;
        var todayDayOfWeekNumber = ((int)DateTime.Now.DayOfWeek + 6) % 7; // Adjusting index for DayOfWeek starting with Sunday
        _datePickerDayOfWeek.SelectedIndex = todayDayOfWeekNumber;

        _timeRadioButton.IsChecked = _time is not null;
        _timePickerPanel.IsEnabled = _time is not null;

        _dateRadioButton.IsChecked = _day is not null;
        _datePickerPanel.IsEnabled = _day is not null;
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
        Day? day = _datePickerPanel.IsEnabled ? GetUserDefinedDay() : null;
        return _trigger.DeepCopy(isSunrise, isSunset, time, day);
    }

    private Day GetUserDefinedDay()
    {
        var selectedComboBox = _datePickerDayOfWeek.SelectedItem as ComboBoxItem;
        var selectedDay = selectedComboBox!.Content.ToString();
        var dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), selectedDay!);
        var dayUtc = DateTimeExtensions.LocalDayFrom(dayOfWeek, (int)_datePickerHours.Value, (int)_datePickerMinutes.Value);
        var day = dayUtc.ToUniversalTime();
        return new Day { Hour = day.Hour, Minute = day.Minute, DayOfWeek = day.DayOfWeek };
    }

    private void RadioButton_Click(object sender, RoutedEventArgs e)
    {
        _timePickerPanel.IsEnabled = _timeRadioButton.IsChecked ?? false;
        _datePickerPanel.IsEnabled = _dateRadioButton.IsChecked ?? false;
    }

    private void _timeRadioButton_Checked(object sender, RoutedEventArgs e)
    {

    }

        private void _dateRadioButton_Checked(object sender, RoutedEventArgs e)
    {

    }
}
