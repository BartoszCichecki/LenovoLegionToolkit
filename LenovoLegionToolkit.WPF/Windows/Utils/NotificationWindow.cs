using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Utils;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Point = System.Drawing.Point;

#pragma warning disable CA1416 // Validate platform compatibility

namespace LenovoLegionToolkit.WPF.Windows.Utils;

public class NotificationWindow : UiWindow
{
    private readonly Grid _mainGrid = new()
    {
        ColumnDefinitions =
        {
            new() { Width = GridLength.Auto, },
            new() { Width = new(1, GridUnitType.Star) },
        },
        Margin = new(16, 16, 32, 16),
    };

    private readonly SymbolIcon _symbolIcon = new()
    {
        FontSize = 32,
        Margin = new(0, 0, 16, 0),
    };

    private readonly SymbolIcon _overlaySymbolIcon = new()
    {
        FontSize = 32,
        Margin = new(0, 0, 16, 0),
    };

    private readonly Label _textBlock = new()
    {
        FontSize = 16,
        FontWeight = FontWeights.Medium,
        VerticalContentAlignment = VerticalAlignment.Center,
    };

    public NotificationWindow(SymbolRegular symbol, SymbolRegular? overlaySymbol, Action<SymbolIcon>? symbolTransform, string text, NotificationPosition position)
    {
        InitializeStyle();
        InitializeContent(symbol, overlaySymbol, symbolTransform, text);

        SourceInitialized += (_, _) => InitializePosition(position);
        MouseDown += (_, _) => Close();
    }

    public void Show(int closeAfter)
    {
        Show();
        Task.Delay(closeAfter).ContinueWith(_ =>
        {
            Close();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private Rect GetPrimaryDesktopWorkingArea()
    {
        var screen = PInvoke.MonitorFromPoint(Point.Empty, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);

        var monitorInfo = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
        if (!PInvoke.GetMonitorInfo(screen, ref monitorInfo))
            return SystemParameters.WorkArea;

        if (!PInvoke.GetDpiForMonitor(screen, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY).Succeeded)
            return SystemParameters.WorkArea;

        var workArea = monitorInfo.rcWork;
        var multiplierX = 96d / dpiX;
        var multiplierY = 96d / dpiY;

        return new Rect(workArea.X, workArea.Y, workArea.Width * multiplierX, workArea.Height * multiplierY);
    }

    private void InitializeStyle()
    {
        WindowStartupLocation = WindowStartupLocation.Manual;
        ResizeMode = ResizeMode.NoResize;
        WindowBackdropType = BackgroundType.None;

        Focusable = false;
        Topmost = true;
        ExtendsContentIntoTitleBar = true;
        ShowInTaskbar = false;
        ShowActivated = false;

        _mainGrid.FlowDirection = LocalizationHelper.Direction;
        _textBlock.Foreground = (SolidColorBrush)FindResource("TextFillColorPrimaryBrush");
    }

    private void InitializePosition(NotificationPosition position)
    {
        var desktopWorkingArea = GetPrimaryDesktopWorkingArea();

        _mainGrid.Measure(new Size(double.PositiveInfinity, 80));

        Width = MinWidth = Math.Max(_mainGrid.DesiredSize.Width, 300);
        Height = MinHeight = _mainGrid.DesiredSize.Height;

        const int margin = 16;

        switch (position)
        {
            case NotificationPosition.BottomRight:
                Left = desktopWorkingArea.Right - Width - margin;
                Top = desktopWorkingArea.Bottom - Height - margin;
                break;
            case NotificationPosition.BottomCenter:
                Left = (desktopWorkingArea.Right - Width) / 2;
                Top = desktopWorkingArea.Bottom - Height - margin;
                break;
            case NotificationPosition.BottomLeft:
                Left = desktopWorkingArea.Left + margin;
                Top = desktopWorkingArea.Bottom - Height - margin;
                break;
            case NotificationPosition.CenterLeft:
                Left = desktopWorkingArea.Left + margin;
                Top = (desktopWorkingArea.Bottom - Height) / 2;
                break;
            case NotificationPosition.TopLeft:
                Left = desktopWorkingArea.Left + margin;
                Top = desktopWorkingArea.Top + margin;
                break;
            case NotificationPosition.TopCenter:
                Left = (desktopWorkingArea.Right - Width) / 2;
                Top = desktopWorkingArea.Top + margin;
                break;
            case NotificationPosition.TopRight:
                Left = desktopWorkingArea.Right - Width - margin;
                Top = desktopWorkingArea.Top + margin;
                break;
            case NotificationPosition.CenterRight:
                Left = desktopWorkingArea.Right - Width - margin;
                Top = (desktopWorkingArea.Bottom - Height) / 2;
                break;
            case NotificationPosition.Center:
                Left = (desktopWorkingArea.Right - Width) / 2;
                Top = (desktopWorkingArea.Bottom - Height) / 2;
                break;
        }
    }

    private void InitializeContent(SymbolRegular symbol, SymbolRegular? overlaySymbol, Action<SymbolIcon>? symbolTransform, string text)
    {
        _symbolIcon.Symbol = symbol;
        _textBlock.Content = text;

        Grid.SetColumn(_symbolIcon, 0);
        Grid.SetColumn(_textBlock, 1);

        _mainGrid.Children.Add(_symbolIcon);
        _mainGrid.Children.Add(_textBlock);

        if (overlaySymbol.HasValue)
        {
            _overlaySymbolIcon.Symbol = overlaySymbol.Value;
            Grid.SetColumn(_overlaySymbolIcon, 0);
            _mainGrid.Children.Add(_overlaySymbolIcon);
        }

        symbolTransform?.Invoke(_symbolIcon);

        Content = _mainGrid;
    }
}