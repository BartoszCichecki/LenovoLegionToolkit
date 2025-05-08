using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Utils;

public class NotificationWindow : UiWindow, INotificationWindow
{
    private readonly ScreenInfo _screenInfo;

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

    private bool _gettingBitMap;

    public NotificationWindow(SymbolRegular symbol, SymbolRegular? overlaySymbol, Action<SymbolIcon>? symbolTransform, string text, Action? clickAction, ScreenInfo screenInfo, NotificationPosition position)
    {
        InitializeStyle();
        InitializeContent(symbol, overlaySymbol, symbolTransform, text);

        _screenInfo = screenInfo;

        SourceInitialized += (_, _) => InitializePosition(screenInfo.WorkArea, screenInfo.DpiX, screenInfo.DpiY, position);
        MouseDown += (_, _) =>
        {
            Close();
            clickAction?.Invoke();
        };
    }

    public void Show(int closeAfter)
    {
        Show();
        Task.Delay(closeAfter).ContinueWith(_ =>
        {
            Close();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void Close(bool immediate)
    {
        WindowStyle = WindowStyle.None;
        Close();
    }

    public Bitmap GetBitmapView()
    {
        _gettingBitMap = true;
        Show();
        Close();
        _gettingBitMap = false;

        RenderTargetBitmap rtb = new((int)Width, (int)Height, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(this);
        var ms = new MemoryStream();
        var encoder = new BmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(rtb));
        encoder.Save(ms);
        using var bitmap = new Bitmap(ms);

        var multiplierX = _screenInfo.DpiX / 96d;
        var multiplierY = _screenInfo.DpiY / 96d;
        var newWidth = (int)(bitmap.Width * multiplierX);
        var newHeight = (int)(bitmap.Height * multiplierY);
        var resizedBitmap = new Bitmap(newWidth, newHeight);
        using var graphics = Graphics.FromImage(resizedBitmap);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var borderPath = GetRoundedRectanglePath(new(0, 0, newWidth, newHeight), 10);
        var penPath = GetRoundedRectanglePath(new(1, 1, newWidth - 3, newHeight - 3), 10);

        graphics.SetClip(borderPath);
        graphics.DrawImage(bitmap, 0, 0, newWidth, newHeight);
        graphics.ResetClip();

        using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(64, 64, 64), 3);
        graphics.DrawPath(pen, penPath);

        return resizedBitmap;
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

    private void InitializePosition(Rect workArea, uint dpiX, uint dpiY, NotificationPosition position)
    {
        _mainGrid.Measure(new System.Windows.Size(double.PositiveInfinity, 80));

        var multiplierX = dpiX / 96d;
        var multiplierY = dpiY / 96d;
        Rect nativeWorkArea = new(workArea.Left, workArea.Top, workArea.Width * multiplierX, workArea.Height * multiplierY);

        Width = MaxWidth = MinWidth = Math.Max(_mainGrid.DesiredSize.Width, 300);
        Height = MaxHeight = MinHeight = _mainGrid.DesiredSize.Height;

        double nativeLeft = 0;
        double nativeTop = 0;

        if (_gettingBitMap)
        {
            nativeLeft = -1048576;
            nativeTop = -1048576;
        }
        else
        {
            var nativeWidth = Width * multiplierX;
            var nativeHeight = Height * multiplierY;

            const int margin = 16;
            var nativeMarginX = margin * multiplierX;
            var nativeMarginY = margin * multiplierY;

            switch (position)
            {
                case NotificationPosition.BottomRight:
                    nativeLeft = nativeWorkArea.Right - nativeWidth - nativeMarginX;
                    nativeTop = nativeWorkArea.Bottom - nativeHeight - nativeMarginY;
                    break;
                case NotificationPosition.BottomCenter:
                    nativeLeft = nativeWorkArea.Left + (nativeWorkArea.Width - nativeWidth) / 2;
                    nativeTop = nativeWorkArea.Bottom - nativeHeight - nativeMarginY;
                    break;
                case NotificationPosition.BottomLeft:
                    nativeLeft = nativeWorkArea.Left + nativeMarginX;
                    nativeTop = nativeWorkArea.Bottom - nativeHeight - nativeMarginY;
                    break;
                case NotificationPosition.CenterLeft:
                    nativeLeft = nativeWorkArea.Left + nativeMarginX;
                    nativeTop = nativeWorkArea.Top + (nativeWorkArea.Height - nativeHeight) / 2;
                    break;
                case NotificationPosition.TopLeft:
                    nativeLeft = nativeWorkArea.Left + nativeMarginX;
                    nativeTop = nativeWorkArea.Top + nativeMarginY;
                    break;
                case NotificationPosition.TopCenter:
                    nativeLeft = nativeWorkArea.Left + (nativeWorkArea.Width - nativeWidth) / 2;
                    nativeTop = nativeWorkArea.Top + nativeMarginY;
                    break;
                case NotificationPosition.TopRight:
                    nativeLeft = nativeWorkArea.Right - nativeWidth - nativeMarginX;
                    nativeTop = nativeWorkArea.Top + nativeMarginY;
                    break;
                case NotificationPosition.CenterRight:
                    nativeLeft = nativeWorkArea.Right - nativeWidth - nativeMarginX;
                    nativeTop = nativeWorkArea.Top + (nativeWorkArea.Height - nativeHeight) / 2;
                    break;
                case NotificationPosition.Center:
                    nativeLeft = nativeWorkArea.Left + (nativeWorkArea.Width - nativeWidth) / 2;
                    nativeTop = nativeWorkArea.Top + (nativeWorkArea.Height - nativeHeight) / 2;
                    break;
            }
        }

        var windowInteropHandler = new WindowInteropHelper(this);

        PInvoke.SetWindowPos((HWND)windowInteropHandler.Handle, HWND.Null, (int)nativeLeft, (int)nativeTop, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
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

    private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}
