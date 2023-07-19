using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using ToolTip = System.Windows.Controls.ToolTip;

namespace LenovoLegionToolkit.WPF.Utils;

public class NotifyIcon : NativeWindow, IDisposable
{
    private const uint TRAY_MESSAGE_ID = PInvoke.WM_USER + 1069;

    private static readonly uint TaskbarCreatedMessage = PInvoke.RegisterWindowMessage("TaskbarCreated");

    private static uint _nextId;

    private readonly object _lock = new();
    private readonly uint _id = ++_nextId;

    private bool _added;

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

    private ToolTip? _toolTip;
    public ToolTip? ToolTip
    {
        set
        {
            _toolTip = value;
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
                        ShowToolTip();
                        break;
                    case PInvoke.NIN_POPUPCLOSE:
                        HideToolTip();
                        break;
                    case PInvoke.WM_LBUTTONUP:
                        HideToolTip();
                        HideContextMenu();
                        OnClick?.Invoke(this, EventArgs.Empty);
                        break;
                    case PInvoke.WM_RBUTTONUP:
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

    private void ShowToolTip()
    {
        if (ContextMenu is not null && ContextMenu.IsOpen)
            return;

        if (_toolTip is null)
            return;

        _toolTip.IsOpen = true;

        if (PresentationSource.FromVisual(_toolTip) is HwndSource source && source.Handle != IntPtr.Zero)
            PInvoke.SetForegroundWindow(new HWND(source.Handle));
    }

    private void HideToolTip()
    {
        if (_toolTip is null || !_toolTip.IsOpen)
            return;

        _toolTip.IsOpen = false;
    }

    private void ShowContextMenu()
    {
        if (ContextMenu is null)
            return;

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
                uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE,
                Anonymous = new() { uVersion = 4 }
            };

            if (_visible && Handle == IntPtr.Zero)
                CreateHandle(new CreateParams());

            data.hWnd = new HWND(Handle);

            if (_icon is not null)
            {
                data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_ICON;
                data.hIcon = new HICON(_icon.Handle);
            }

            if (_text is not null && _toolTip is null)
            {
                data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_TIP | NOTIFY_ICON_DATA_FLAGS.NIF_SHOWTIP;
                data.szTip = _text;
            }

            switch (_visible, _added)
            {
                case (true, false):
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, data);
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
        _toolTip = null;
        ContextMenu = null;

        ReleaseHandle();
    }
}
