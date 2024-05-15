using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Macro;

namespace LenovoLegionToolkit.WPF.Controls.Macro;

public partial class MacroSequenceControl
{
    private readonly MacroController _controller = IoCContainer.Resolve<MacroController>();

    private ulong _key;

    public MacroSequenceControl()
    {
        InitializeComponent();

        _controller.RecorderReceived += Controller_RecorderReceived;
    }

    public void Set(ulong key)
    {
        _key = key;

        var sequence = _controller.GetSequences().GetValueOrDefault(key);

        _macroEventsPanel.Children.Clear();
        _recordButton.Visibility = Visibility.Visible;
        _stopRecordingButton.Visibility = Visibility.Collapsed;
        _clearButton.Visibility = sequence.Events?.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        _ignoreDelaysToggle.IsChecked = sequence.IgnoreDelays;
        foreach (var macroEvent in sequence.Events ?? [])
            CreateControl(macroEvent);
    }

    private void CreateControl(MacroEvent macroEvent)
    {
        var macroEventControl = new MacroEventControl();
        macroEventControl.Set(macroEvent);
        _macroEventsPanel.Children.Add(macroEventControl);
    }

    private void Controller_RecorderReceived(object? sender, MacroController.RecorderReceivedEventArgs e) => CreateControl(e.MacroEvent);

    private void RecordButton_Click(object sender, RoutedEventArgs e)
    {
        _macroEventsPanel.Children.Clear();
        _recordButton.Visibility = Visibility.Collapsed;
        _stopRecordingButton.Visibility = Visibility.Visible;
        _clearButton.Visibility = Visibility.Collapsed;

        Mouse.OverrideCursor = Cursors.AppStarting;

        _controller.StartRecording();
    }

    private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
    {
        _controller.StopRecording();

        Mouse.OverrideCursor = null;

        var ignoreDelays = _ignoreDelaysToggle.IsChecked ?? false;
        var macroEvents = _macroEventsPanel.Children
            .OfType<MacroEventControl>()
            .Select(c => c.MacroEvent)
            .ToArray();

        _recordButton.Visibility = Visibility.Visible;
        _stopRecordingButton.Visibility = Visibility.Collapsed;
        _clearButton.Visibility = macroEvents.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        var sequences = _controller.GetSequences();
        sequences[_key] = new MacroSequence
        {
            IgnoreDelays = ignoreDelays,
            RepeatCount = 1,
            Events = macroEvents
        };
        _controller.SetSequences(sequences);
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _macroEventsPanel.Children.Clear();

        var sequences = _controller.GetSequences();
        sequences.Remove(_key);
        _controller.SetSequences(sequences);

    }
}