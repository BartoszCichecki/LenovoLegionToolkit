using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Macro.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Macro;

public class MacroController
{
    public class RecorderReceivedEventArgs : EventArgs
    {
        public MacroEvent MacroEvent { get; init; }
    }

    private static readonly uint[] AllowedKeys = [0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69];
    public static readonly int[] AllowedRepeatCounts = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    private readonly MacroRecorder _recorder = new();
    private readonly MacroPlayer _player = new();

    private readonly HOOKPROC _kbProc;
    private readonly MacroSettings _settings;

    private HHOOK _kbHook;
    private CancellationTokenSource? _cancellationTokenSource;

    public event EventHandler<RecorderReceivedEventArgs>? RecorderReceived;

    public MacroController(MacroSettings settings)
    {
        _settings = settings;

        _kbProc = LowLevelKeyboardProc;

        _recorder.Received += Recorder_Received;
    }

    private void Recorder_Received(object? sender, MacroRecorder.ReceivedEventArgs e) => RecorderReceived?.Invoke(this, new() { MacroEvent = e.MacroEvent });

    public bool IsEnabled => _settings.Store.IsEnabled;

    public void SetEnabled(bool enabled)
    {
        _settings.Store.IsEnabled = enabled;
        _settings.SynchronizeStore();
    }

    public Dictionary<MacroIdentifier, MacroSequence> GetSequences() => _settings.Store.Sequences;

    public void SetSequences(Dictionary<MacroIdentifier, MacroSequence> sequences)
    {
        _settings.Store.Sequences = sequences;
        _settings.SynchronizeStore();
    }

    public void Start()
    {
        if (_kbHook != default)
            return;

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _kbProc, HINSTANCE.Null, 0);
    }

    public void StartRecording() => _recorder.StartRecording();

    public void StopRecording() => _recorder.StopRecording();

    private unsafe LRESULT LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode != PInvoke.HC_ACTION)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        if (!IsEnabled)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        ref var kbStruct = ref Unsafe.AsRef<KBDLLHOOKSTRUCT>((void*)lParam.Value);

        var shouldRun = !_recorder.IsRecording;
        shouldRun &= kbStruct.flags == 0;
        shouldRun &= AllowedKeys.Contains(kbStruct.vkCode);
        shouldRun &= _settings.Store.Sequences.GetValueOrNull(new(MacroSource.Keyboard, kbStruct.vkCode))?.Events?.Length > 0;

        if (!shouldRun)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        var sequence = _settings.Store.Sequences[new(MacroSource.Keyboard, kbStruct.vkCode)];

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        Play(sequence, token);

        // Returning a value greater than zero to prevent other hooks from handling the keypress
        return new LRESULT(96);
    }

    private void Play(MacroSequence sequence, CancellationToken token) => Task.Run(() => _player.PlayAsync(sequence, token), token);
}
