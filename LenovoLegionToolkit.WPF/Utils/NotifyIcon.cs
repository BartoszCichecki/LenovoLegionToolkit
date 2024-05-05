using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Interop;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Utils;

public class NotifyIcon : NativeWindow, IDisposable
{
    private const uint TRAY_MESSAGE_ID = PInvoke.WM_USER + 1069;

    private static readonly uint TaskbarCreatedMessage = PInvoke.RegisterWindowMessage("TaskbarCreated");

    private static uint _nextId;

    private readonly object _lock = new();
    private readonly uint _id = ++_nextId;

    private bool _added;
    private CancellationTokenSource? _showToolTipCancellationTokenSource;

    private bool _visible;
    public bool Visible
    {
        set
        {
            _visible = value;
            UpdateIcon();
        }
    }

    private Icon? _icon;
    public Icon? Icon
    {
        set
        {
            _icon = value;
            UpdateIcon();

        }
    }

    private string? _text;
    public string? Text
    {
        set
        {
            _text = value;
            UpdateIcon();
        }
    }

    private UiWindow? _currentToolTipWindow;

    private Func<Task<UiWindow>>? _toolTipWindow;
    public Func<Task<UiWindow>>? ToolTipWindow
    {
        set
        {
            _toolTipWindow = value;
            UpdateIcon();
        }
    }

    public ContextMenu? ContextMenu { get; set; }

    public event EventHandler? OnClick;

    public NotifyIcon()
    {
        UpdateIcon();
    }

    protected override void WndProc(ref Message m)
    {
        switch ((uint)m.Msg)
        {
            case TRAY_MESSAGE_ID:
                switch ((uint)m.LParam & 0xFFFF)
                {
                    case PInvoke.NIN_POPUPOPEN:
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"NIN_POPUPOPEN");
                        ShowToolTipAsync();
                        break;
                    case PInvoke.NIN_POPUPCLOSE:
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"NIN_POPUPCLOSE");
                        HideToolTip();
                        break;
                    case PInvoke.WM_LBUTTONUP:
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"WM_LBUTTONUP");
                        HideToolTip();
                        HideContextMenu();
                        OnClick?.Invoke(this, EventArgs.Empty);
                        break;
                    case PInvoke.WM_RBUTTONUP:
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"WM_RBUTTONUP");
                        HideToolTip();
                        ShowContextMenu();
                        break;
                }
                break;
            case PInvoke.WM_DESTROY:
                _visible = false;
                UpdateIcon();
                break;
            default:
                if (m.Msg == TaskbarCreatedMessage && _visible)
                {
                    _visible = true;
                    _added = false;
                    UpdateIcon();
                }

                DefWndProc(ref m);
                break;
        }
    }

    private async void ShowToolTipAsync()
    {
        if (_toolTipWindow is null)
            return;

        if (_showToolTipCancellationTokenSource is not null)
            await _showToolTipCancellationTokenSource.CancelAsync();
        _showToolTipCancellationTokenSource = new();

        var token = _showToolTipCancellationTokenSource.Token;

        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500), token);

            if (ContextMenu is not null && ContextMenu.IsOpen)
                return;

            _currentToolTipWindow?.Close();
            _currentToolTipWindow = await _toolTipWindow();

            token.ThrowIfCancellationRequested();

            _currentToolTipWindow?.Show();
        }
        catch (OperationCanceledException)
        {
            _currentToolTipWindow?.Close();
            _currentToolTipWindow = null;
        }
        catch (Exception ex)
        {
            _currentToolTipWindow?.Close();
            _currentToolTipWindow = null;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to show tooltip.", ex);
        }
    }

    private void HideToolTip()
    {
        if (_toolTipWindow is null)
            return;

        _showToolTipCancellationTokenSource?.Cancel();

        _currentToolTipWindow?.Hide();
        _currentToolTipWindow = null;
    }

    private void ShowContextMenu()
    {
        if (ContextMenu is null)
            return;

        ContextMenu.Placement = PlacementMode.Mouse;
        ContextMenu.PlacementRectangle = Rect.Empty;
        ContextMenu.PlacementTarget = null;
        ContextMenu.IsOpen = true;

        if (PresentationSource.FromVisual(ContextMenu) is HwndSource source && source.Handle != IntPtr.Zero)
            PInvoke.SetForegroundWindow(new HWND(source.Handle));
    }

    private void HideContextMenu()
    {
        if (ContextMenu is null || !ContextMenu.IsOpen)
            return;

        ContextMenu.IsOpen = false;
    }

    private void UpdateIcon()
    {
        lock (_lock)
        {
            var data = new NOTIFYICONDATAW
            {
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW>(),
                uID = _id,
                uCallbackMessage = TRAY_MESSAGE_ID,
                uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE | NOTIFY_ICON_DATA_FLAGS.NIF_TIP,
                szTip = " "
            };

            if (_visible && Handle == IntPtr.Zero)
                CreateHandle(new CreateParams());

            data.hWnd = new HWND(Handle);

            if (_icon is not null)
            {
                data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_ICON;
                data.hIcon = new HICON(_icon.Handle);
            }

            if (_text is not null && _toolTipWindow is null)
            {
                data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_SHOWTIP;
                data.szTip = _text;
            }

            switch (_visible, _added)
            {
                case (true, false):
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, data);
                    data.Anonymous = new() { uVersion = 4 };
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, data);
                    _added = true;
                    break;
                case (true, true):
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_MODIFY, data);
                    break;
                case (false, true):
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_DELETE, data);
                    _added = false;
                    break;
            }
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        HideContextMenu();
        HideToolTip();

        _visible = false;
        UpdateIcon();

        _icon?.Dispose();

        _icon = null;
        _text = null;
        _toolTipWindow = null;
        ContextMenu = null;

        ReleaseHandle();
    }
}
