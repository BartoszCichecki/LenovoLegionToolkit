﻿using System;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard.GodMode;

public partial class GodModeValueControl
{
    private int? _defaultValue;

    public string Title
    {
        get => _cardControlHeader.Title;
        set => _cardControlHeader.Title = value;
    }

    public string Description
    {
        get => _cardControlHeader.Subtitle;
        set => _cardControlHeader.Subtitle = value;
    }

    public string Unit { get; set; } = string.Empty;

    public int Value
    {
        get
        {
            if (_slider.Visibility == Visibility.Visible)
                return (int)_slider.Value;

            if (_comboBox.Visibility == Visibility.Visible && _comboBox.TryGetSelectedItem(out int value))
                return value;

            throw new InvalidOperationException("Unable to get Value");
        }
        set
        {
            if (_slider.Visibility == Visibility.Visible)
            {
                _slider.Value = value;
                return;
            }

            if (_comboBox.Visibility == Visibility.Visible)
            {
                _comboBox.SelectItem(value);
                return;
            }

            throw new InvalidOperationException("Unable to set Value");
        }
    }

    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        add => _slider.ValueChanged += value;
        remove => _slider.ValueChanged -= value;
    }

    public GodModeValueControl() => InitializeComponent();

    public void Set(StepperValue? stepperValue, int? maxValueOffset = 0)
    {
        if (!stepperValue.HasValue)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        var value = stepperValue.Value;

        if (value.Step != 0)
        {
            _slider.Visibility = Visibility.Visible;
            _sliderLabel.Visibility = Visibility.Visible;
            _comboBox.Visibility = Visibility.Collapsed;

            _slider.Minimum = value.Min;
            _slider.Maximum = value.Max + (maxValueOffset ?? 0);
            _slider.TickFrequency = value.Step;
            _slider.Value = value.Value;

            _sliderLabel.ContentStringFormat = $"{0} {Unit}";

            _comboBox.Items.Clear();
            _comboBox.SelectedItem = null;

            _defaultValue = value.DefaultValue;
            _resetToDefaultButton.Visibility = _defaultValue.HasValue ? Visibility.Visible : Visibility.Collapsed;

            Visibility = Visibility.Visible;
            return;
        }

        if (value.Steps.Length > 0)
        {
            _slider.Visibility = Visibility.Collapsed;
            _sliderLabel.Visibility = Visibility.Collapsed;
            _comboBox.Visibility = Visibility.Visible;

            _slider.Minimum = 0;
            _slider.Maximum = 0;
            _slider.TickFrequency = 0;
            _slider.Value = 0;

            _sliderLabel.ContentStringFormat = null;

            _comboBox.SetItems(value.Steps, value.Value, v => $"{v} {Unit}");

            _defaultValue = value.DefaultValue;
            _resetToDefaultButton.Visibility = _defaultValue.HasValue ? Visibility.Visible : Visibility.Collapsed;

            Visibility = Visibility.Visible;
            return;
        }

        Visibility = Visibility.Collapsed;
    }

    private void ResetToDefaultButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!_defaultValue.HasValue)
            return;

        if (_slider.Visibility == Visibility.Visible)
            _slider.Value = _defaultValue.Value;

        if (_comboBox.Visibility == Visibility.Visible)
            _comboBox.SelectItem(_defaultValue.Value);
    }
}