using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Macro;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Windows.Macro;

namespace LenovoLegionToolkit.WPF.Controls.Macro;

public partial class MacroSequenceControl
{
    private readonly MacroController _controller = IoCContainer.Resolve<MacroController>();

    private MacroRecordingWindow? _recordingWindow;
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
        _interruptOnOtherKeyCard.IsEnabled = sequenceHasEvents;

        _settingsComboBox.SetItems([MacroRecorderSettings.Keyboard, MacroRecorderSettings.Keyboard | MacroRecorderSettings.Mouse, MacroRecorderSettings.Keyboard | MacroRecorderSettings.Mouse | MacroRecorderSettings.Movement],
            MacroRecorderSettings.Keyboard,
            v => v switch
            {
                MacroRecorderSettings.Keyboard => Resource.MacroSequenceControl_Keyboard,
                MacroRecorderSettings.Keyboard | MacroRecorderSettings.Mouse => Resource.MacroSequenceControl_KeyboardMouse,
                MacroRecorderSettings.Keyboard | MacroRecorderSettings.Mouse | MacroRecorderSettings.Movement => Resource.MacroSequenceControl_KeyboardMouseMovement,
                _ => string.Empty
            });
        _repeatComboBox.SetItems(MacroController.AllowedRepeatCounts,
            Math.Clamp(sequence.RepeatCount, 1, 10),
            v => v == 1 ? Resource.MacroSequenceControl_DontRepeat : v.ToString());
        _ignoreDelaysToggle.IsChecked = sequence.IgnoreDelays;
        _interruptOnOtherKeyToggle.IsChecked = sequence.InterruptOnOtherKey;

        _recordButton.IsEnabled = true;
        _clearButton.Visibility = sequenceHasEvents ? Visibility.Visible : Visibility.Collapsed;

        _macroEventsPanel.Children.Clear();
        foreach (var macroEvent in sequence.Events ?? [])
            CreateControl(macroEvent);

        _isRefreshing = false;
    }

    private void CreateControl(MacroEvent macroEvent)
    {
        if (macroEvent.Direction is MacroDirection.Move)
        {
            if (_macroEventsPanel.Children.OfType<AbstractMacroEventControl>().LastOrDefault() is MultiAbstractMacroEventControl last)
            {
                last.Set(macroEvent);
            }
            else
            {
                var macroEventControl = new MultiAbstractMacroEventControl();
                macroEventControl.Set(macroEvent);
                _macroEventsPanel.Children.Add(macroEventControl);
            }
        }
        else
        {
            var macroEventControl = new SingleAbstractMacroEventControl();
            macroEventControl.Set(macroEvent);
            _macroEventsPanel.Children.Add(macroEventControl);
        }
    }

    private void Controller_RecorderReceived(object? sender, MacroController.RecorderReceivedEventArgs e) => CreateControl(e.MacroEvent);

    private void Controller_RecorderStopped(object? sender, MacroController.RecorderStoppedEventArgs e)
    {
        _recordingWindow?.Close();
        _recordingWindow = null;

        if (e.Interrupted)
            Clear();
        else
            Save();
    }

    private void RepeatComboBox_SelectionChanged(object sender, RoutedEventArgs e) => Save();

    private void IgnoreDelaysToggle_Click(object sender, RoutedEventArgs e) => Save();

    private void InterruptOnOtherKeyToggle_Click(object sender, RoutedEventArgs e) => Save();

    private void ClearButton_Click(object sender, RoutedEventArgs e) => Clear();

    private async void RecordButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_settingsComboBox.TryGetSelectedItem(out MacroRecorderSettings settings))
            return;

        await RecordAsync(settings);
    }

    private async Task RecordAsync(MacroRecorderSettings settings)
    {
        _macroEventsPanel.Children.Clear();
        _recordButton.IsEnabled = false;
        _clearButton.Visibility = Visibility.Collapsed;

        Mouse.OverrideCursor = Cursors.Wait;

        if (settings.HasFlag(MacroRecorderSettings.Mouse) && settings.HasFlag(MacroRecorderSettings.Movement))
        {
            _recordingWindow = MacroRecordingWindow.CreatePreparing();
            _recordingWindow.Owner = Window.GetWindow(this);
            _recordingWindow.Show();

            await Task.Delay(TimeSpan.FromSeconds(3));

            _recordingWindow.Close();
        }

        _recordingWindow = MacroRecordingWindow.CreateRecording();
        _recordingWindow.Owner = Window.GetWindow(this);
        _recordingWindow.Show();

        _controller.StartRecording(settings);
    }

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
        var interruptOnOtherKey = _interruptOnOtherKeyToggle.IsChecked ?? false;
        var macroEvents = _macroEventsPanel.Children
            .OfType<AbstractMacroEventControl>()
            .SelectMany(c => c.GetEvents())
            .ToArray();

        var sequences = _controller.GetSequences();
        sequences[_macroIdentifier] = new MacroSequence
        {
            RepeatCount = repeatCount,
            IgnoreDelays = ignoreDelays,
            InterruptOnOtherKey = interruptOnOtherKey,
            Events = macroEvents
        };
        _controller.SetSequences(sequences);

        Set(_macroIdentifier);
    }
}
