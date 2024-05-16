using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Macro;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Controls.Macro;

public partial class MacroSequenceControl
{
    private readonly MacroController _controller = IoCContainer.Resolve<MacroController>();

    private MacroIdentifier _macroIdentifier;
    private bool _isRefreshing;

    public MacroSequenceControl()
    {
        InitializeComponent();

        _controller.RecorderReceived += Controller_RecorderReceived;
    }

    public void Set(MacroIdentifier macroIdentifier)
    {
        _isRefreshing = true;

        _macroIdentifier = macroIdentifier;

        _controller.StopRecording();

        Mouse.OverrideCursor = null;

        var sequence = _controller.GetSequences().GetValueOrDefault(_macroIdentifier);

        _macroEventsPanel.Children.Clear();
        _recordButton.Visibility = Visibility.Visible;
        _stopRecordingButton.Visibility = Visibility.Collapsed;
        _clearButton.Visibility = sequence.Events?.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        _repeatComboBox.SetItems(MacroController.AllowedRepeatCounts,
            Math.Clamp(sequence.RepeatCount, 1, 10),
            v => v == 1 ? Resource.MacroSequenceControl_DontRepeat : v.ToString());
        _ignoreDelaysToggle.IsChecked = sequence.IgnoreDelays;
        foreach (var macroEvent in sequence.Events ?? [])
            CreateControl(macroEvent);

        _isRefreshing = false;
    }

    private void CreateControl(MacroEvent macroEvent)
    {
        var macroEventControl = new MacroEventControl();
        macroEventControl.Set(macroEvent);
        _macroEventsPanel.Children.Add(macroEventControl);
    }

    private void Controller_RecorderReceived(object? sender, MacroController.RecorderReceivedEventArgs e) => CreateControl(e.MacroEvent);

    private void IgnoreDelaysToggle_Click(object sender, RoutedEventArgs e) => Save();

    private void RepeatComboBox_SelectionChanged(object sender, RoutedEventArgs e) => Save();

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _macroEventsPanel.Children.Clear();

        Save();
    }

    private void RecordButton_Click(object sender, RoutedEventArgs e)
    {
        _macroEventsPanel.Children.Clear();
        _recordButton.Visibility = Visibility.Collapsed;
        _stopRecordingButton.Visibility = Visibility.Visible;
        _clearButton.Visibility = Visibility.Collapsed;

        Mouse.OverrideCursor = Cursors.AppStarting;

        _controller.StartRecording();
    }

    private void StopRecordingButton_Click(object sender, RoutedEventArgs e) => Save();

    private void Save()
    {
        if (_isRefreshing)
            return;

        _controller.StopRecording();

        Mouse.OverrideCursor = null;

        var repeatCount = _repeatComboBox.TryGetSelectedItem(out int repeat) ? repeat : 1;
        var ignoreDelays = _ignoreDelaysToggle.IsChecked ?? false;
        var macroEvents = _macroEventsPanel.Children
            .OfType<MacroEventControl>()
            .Select(c => c.MacroEvent)
            .ToArray();

        _recordButton.Visibility = Visibility.Visible;
        _stopRecordingButton.Visibility = Visibility.Collapsed;
        _clearButton.Visibility = macroEvents.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        var sequences = _controller.GetSequences();
        sequences[_macroIdentifier] = new MacroSequence
        {
            IgnoreDelays = ignoreDelays,
            RepeatCount = repeatCount,
            Events = macroEvents
        };
        _controller.SetSequences(sequences);
    }
}
