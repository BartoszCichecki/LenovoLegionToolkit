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
        _controller.RecorderStopped += Controller_RecorderStopped;
    }

    public void Set(MacroIdentifier macroIdentifier)
    {
        _isRefreshing = true;

        _macroIdentifier = macroIdentifier;

        _controller.StopRecording();

        Mouse.OverrideCursor = null;

        var sequence = _controller.GetSequences().GetValueOrDefault(_macroIdentifier);
        var sequenceHasEvents = sequence.Events?.Length > 0;

        _repeatCard.IsEnabled = sequenceHasEvents;
        _ignoreDelaysCard.IsEnabled = sequenceHasEvents;

        _repeatComboBox.SetItems(MacroController.AllowedRepeatCounts,
            Math.Clamp(sequence.RepeatCount, 1, 10),
            v => v == 1 ? Resource.MacroSequenceControl_DontRepeat : v.ToString());
        _ignoreDelaysToggle.IsChecked = sequence.IgnoreDelays;

        _recordButton.Visibility = Visibility.Visible;
        _stopRecordingButton.Visibility = Visibility.Collapsed;
        _clearButton.Visibility = sequenceHasEvents ? Visibility.Visible : Visibility.Collapsed;

        _macroEventsPanel.Children.Clear();
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

    private void Controller_RecorderStopped(object? sender, MacroController.RecorderStoppedEventArgs e)
    {
        if (e.Interrupted)
            Clear();
        else
            Save();
    }

    private void IgnoreDelaysToggle_Click(object sender, RoutedEventArgs e) => Save();

    private void RepeatComboBox_SelectionChanged(object sender, RoutedEventArgs e) => Save();

    private void ClearButton_Click(object sender, RoutedEventArgs e) => Clear();

    private void RecordButton_Click(object sender, RoutedEventArgs e)
    {
        _macroEventsPanel.Children.Clear();
        _recordButton.Visibility = Visibility.Collapsed;
        _stopRecordingButton.Visibility = Visibility.Visible;
        _clearButton.Visibility = Visibility.Collapsed;

        Mouse.OverrideCursor = Cursors.AppStarting;

        _controller.StartRecording();
    }

    private void StopRecordingButton_Click(object sender, RoutedEventArgs e) => _controller.StopRecording();

    private void Clear()
    {
        _macroEventsPanel.Children.Clear();

        Save();
    }

    private void Save()
    {
        if (_isRefreshing)
            return;

        Mouse.OverrideCursor = null;

        var repeatCount = _repeatComboBox.TryGetSelectedItem(out int repeat) ? repeat : 1;
        var ignoreDelays = _ignoreDelaysToggle.IsChecked ?? false;
        var macroEvents = _macroEventsPanel.Children
            .OfType<MacroEventControl>()
            .Select(c => c.MacroEvent)
            .ToArray();

        var sequences = _controller.GetSequences();
        sequences[_macroIdentifier] = new MacroSequence
        {
            IgnoreDelays = ignoreDelays,
            RepeatCount = repeatCount,
            Events = macroEvents
        };
        _controller.SetSequences(sequences);

        Set(_macroIdentifier);
    }
}
