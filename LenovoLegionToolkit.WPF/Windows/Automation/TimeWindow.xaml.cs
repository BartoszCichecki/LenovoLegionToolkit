using System;
using System.Windows;
using LenovoLegionToolkit.Lib.Extensions;
using Time = LenovoLegionToolkit.Lib.Time;

namespace LenovoLegionToolkit.WPF.Windows.Automation
{
    public partial class TimeWindow
    {
        public event EventHandler<(bool, bool, Time?)>? OnSave;

        public TimeWindow(bool isSunrise, bool isSunset, Time? time)
        {
            InitializeComponent();

            _sunriseRadioButton.IsChecked = isSunrise;
            _sunsetRadioButton.IsChecked = isSunset;

            DateTime local;
            if (time is not null)
                local = DateTimeExtensions.UtcFrom(time.Value.Hour, time.Value.Minute).ToLocalTime();
            else
                local = DateTime.Now;

            _timePickerHours.Value = local.Hour;
            _timePickerMinutes.Value = local.Minute;

            _timeRadioButton.IsChecked = time is not null;
            _timePickerPanel.IsEnabled = time is not null;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            _timePickerPanel.IsEnabled = _timeRadioButton.IsChecked ?? false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var isSunrise = _sunriseRadioButton.IsChecked ?? false;
            var isSunset = _sunsetRadioButton.IsChecked ?? false;

            var utc = DateTimeExtensions.LocalFrom((int)_timePickerHours.Value, (int)_timePickerMinutes.Value).ToUniversalTime();
            Time? time = _timePickerPanel.IsEnabled ? new()
            {
                Hour = utc.Hour,
                Minute = utc.Minute,
            } : null;

            OnSave?.Invoke(this, (isSunrise, isSunset, time));
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
