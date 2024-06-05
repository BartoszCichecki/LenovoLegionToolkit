using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class TimeAutomationPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<ITimeAutomationPipelineTrigger>
{
    private readonly ITimeAutomationPipelineTrigger _trigger;
    private readonly bool _isSunrise;
    private readonly bool _isSunset;
    private readonly Time? _time;
    private readonly DayOfWeek[] _days;

    public TimeAutomationPipelineTriggerTabItemContent(ITimeAutomationPipelineTrigger trigger)
    {
        _trigger = trigger;
        _isSunrise = trigger.IsSunrise;
        _isSunset = trigger.IsSunset;
        _time = trigger.Time;
        _days = trigger.Days;

        InitializeComponent();
        UpdateCheckBoxes();
    }

    private void UpdateCheckBoxes()
    {
        foreach (var checkBox in _daysOfWeekPanel.Children.OfType<CheckBox>())
            checkBox.Content = Resource.Culture.DateTimeFormat.GetDayName((DayOfWeek)checkBox.Tag);
    }

    private void TimeAutomationPipelineTriggerTabItemContent_Initialized(object? sender, EventArgs e)
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

        var daysOfWeek = _days.Length != 0 ? _days : Enum.GetValues<DayOfWeek>();
        foreach (var daysOfWeekCheckbox in _daysOfWeekPanel.Children.OfType<CheckBox>())
        {
            if (daysOfWeek.Contains((DayOfWeek)daysOfWeekCheckbox.Tag))
                daysOfWeekCheckbox.IsChecked = true;
        }
    }

    public ITimeAutomationPipelineTrigger GetTrigger()
    {
        var isSunrise = _sunriseRadioButton.IsChecked ?? false;
        var isSunset = _sunsetRadioButton.IsChecked ?? false;
        var time = GetSelectedTime();
        var days = GetSelectedDays();

        return _trigger.DeepCopy(isSunrise, isSunset, time, days);
    }

    private Time? GetSelectedTime()
    {
        if (!_timePickerPanel.IsEnabled)
            return null;

        var pickedHour = (int?)_timePickerHours.Value ?? 0;
        var pickedMinute = (int?)_timePickerMinutes.Value ?? 0;

        var utc = DateTimeExtensions.LocalFrom(pickedHour, pickedMinute).ToUniversalTime();
        return new Time(utc.Hour, utc.Minute);
    }

    private DayOfWeek[] GetSelectedDays()
    {
        var days = _daysOfWeekPanel.Children
            .OfType<CheckBox>()
            .Where(c => c.IsChecked == true)
            .Select(c => c.Tag)
            .Cast<DayOfWeek>()
            .ToArray();

        if (days.IsEmpty())
            days = Enum.GetValues<DayOfWeek>();

        return days;
    }

    private void RadioButton_Click(object sender, RoutedEventArgs e)
    {
        _timePickerPanel.IsEnabled = _timeRadioButton.IsChecked ?? false;
    }

    private void DayOfWeekCheckBox_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkBox)
            return;

        var anySelected = _daysOfWeekPanel.Children
            .OfType<CheckBox>()
            .Any(c => c.IsChecked == true);

        if (anySelected)
            return;

        checkBox.IsChecked = true;
    }
}
