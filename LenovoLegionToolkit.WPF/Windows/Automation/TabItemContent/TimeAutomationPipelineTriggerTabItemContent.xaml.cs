using System;
using System.Collections.Generic;
using System.Globalization;
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
    private readonly DayOfWeek[] _days;

    public TimeAutomationPipelineTriggerTabItemContent(ITimeAutomationPipelineTrigger trigger)
    {
        _trigger = trigger;
        _isSunrise = trigger.IsSunrise;
        _isSunset = trigger.IsSunset;
        _time = trigger.Time;
        _days = trigger.Days;
        InitializeComponent();
        CreateCheckBoxes();
    }

    private void CreateCheckBoxes()
    {
        var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        var localizedAvailableDays = new List<string>();
        var firstDayOfWeekIndex = (int)dateTimeFormat.FirstDayOfWeek;
        for (int i = firstDayOfWeekIndex; localizedAvailableDays.Count < 7; i++)
        {
            var day = (DayOfWeek)(i % 7);
            var localizedDay = dateTimeFormat.GetDayName(day);
            localizedAvailableDays.Add(localizedDay);
        }
        foreach (string localizedAvailableDay in localizedAvailableDays)
        {
            CheckBox dayCheckBox = new CheckBox();
            dayCheckBox.Content = localizedAvailableDay;
            _checkboxContainer.Children.Add(dayCheckBox);
        }
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

        _dayPickerHours.Value = local.Hour;
        _dayPickerMinutes.Value = local.Minute;

        _timeRadioButton.IsChecked = _time is not null;
        _timePickerPanel.IsEnabled = _time is not null && _days.Length == 0;

        _dayRadioButton.IsChecked = _days is not null;
        _dayPickerPanel.IsEnabled = _days is not null;
    }

    public ITimeAutomationPipelineTrigger GetTrigger()
    {
        var isSunrise = _sunriseRadioButton.IsChecked ?? false;
        var isSunset = _sunsetRadioButton.IsChecked ?? false;
        var time = GetSelectedTime();
        DayOfWeek[] days = _dayPickerPanel.IsEnabled ? GetSelectedDays() : new DayOfWeek[0];
        return _trigger.DeepCopy(isSunrise, isSunset, time, days);
    }

    private Time? GetSelectedTime()
    {
        if (!_timePickerPanel.IsEnabled && !_dayPickerPanel.IsEnabled)
            return null;
        int pickedHour, pickedMinute;
        if (_timePickerPanel.IsEnabled == true)
        {
            pickedHour = (int)_timePickerHours.Value;
            pickedMinute = (int)_timePickerMinutes.Value;
        }
        else
        {
            pickedHour = (int)_dayPickerHours.Value;
            pickedMinute = (int)_dayPickerMinutes.Value;
        }
        var utc = DateTimeExtensions
                .LocalFrom(pickedHour, pickedMinute)
                .ToUniversalTime();
        return new Time { Hour = utc.Hour, Minute = utc.Minute };
    }   

    private DayOfWeek[] GetSelectedDays()
    {
        var selectedDays = new List<DayOfWeek>();
        foreach (CheckBox dayCheckBox in _checkboxContainer.Children)
        {
            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            if (dayCheckBox.IsChecked == true)
            {
                var day = (DayOfWeek)Array.IndexOf(dateTimeFormat.DayNames, dayCheckBox.Content.ToString());
                selectedDays.Add(day);
            }
        }
        return selectedDays.ToArray();
    }

    private void RadioButton_Click(object sender, RoutedEventArgs e)
    {
        _timePickerPanel.IsEnabled = _timeRadioButton.IsChecked ?? false;
        _dayPickerPanel.IsEnabled = _dayRadioButton.IsChecked ?? false;
    }

    private void _timeRadioButton_Checked(object sender, RoutedEventArgs e)
    {

    }

        private void _dayRadioButton_Checked(object sender, RoutedEventArgs e)
    {

    }
}
