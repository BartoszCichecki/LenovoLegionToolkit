using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Windows.Utils;

public class NotificationAoTWindow : NativeLayeredWindow, INotificationWindow
{
    private readonly Bitmap _bitmap;
    private readonly ScreenInfo _screenInfo;
    private readonly NotificationPosition _pos;

    public NotificationAoTWindow(Bitmap bitmap, ScreenInfo screenInfo, NotificationPosition position)
    {
        _bitmap = bitmap;
        _screenInfo = screenInfo;
        _pos = position;
        UpdatePosition();
    }

    public void Show(int closeAfter)
    {
        Show();
        Task.Delay(closeAfter).ContinueWith(_ =>
        {
            Close(false);
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    protected override void Paint(PaintEventArgs e)
    {
        e.Graphics.DrawImage(_bitmap, new Rectangle(0, 0, _bitmap.Width, _bitmap.Height));
    }

    private void UpdatePosition()
    {
        var multiplierX = _screenInfo.DpiX / 96d;
        var multiplierY = _screenInfo.DpiY / 96d;
        var wa = _screenInfo.WorkArea;
        Rect workArea = new(wa.Left, wa.Top, wa.Width * multiplierX, wa.Height * multiplierY);

        Size = new(_bitmap.Width, _bitmap.Height);

        const int margin = 16;
        var marginX = margin * multiplierX;
        var marginY = margin * multiplierY;

        double left = 0;
        double top = 0;

        switch (_pos)
        {
            case NotificationPosition.BottomRight:
                left = workArea.Right - Width - marginX;
                top = workArea.Bottom - Height - marginY;
                break;
            case NotificationPosition.BottomCenter:
                left = workArea.Left + (workArea.Width - Width) / 2;
                top = workArea.Bottom - Height - marginY;
                break;
            case NotificationPosition.BottomLeft:
                left = workArea.Left + marginX;
                top = workArea.Bottom - Height - marginY;
                break;
            case NotificationPosition.CenterLeft:
                left = workArea.Left + marginX;
                top = workArea.Top + (workArea.Height - Height) / 2;
                break;
            case NotificationPosition.TopLeft:
                left = workArea.Left + marginX;
                top = workArea.Top + marginY;
                break;
            case NotificationPosition.TopCenter:
                left = workArea.Left + (workArea.Width - Width) / 2;
                top = workArea.Top + marginY;
                break;
            case NotificationPosition.TopRight:
                left = workArea.Right - Width - marginX;
                top = workArea.Top + marginY;
                break;
            case NotificationPosition.CenterRight:
                left = workArea.Right - Width - marginX;
                top = workArea.Top + (workArea.Height - Height) / 2;
                break;
            case NotificationPosition.Center:
                left = workArea.Left + (workArea.Width - Width) / 2;
                top = workArea.Top + (workArea.Height - Height) / 2;
                break;
        }

        Position = new((int)left, (int)top);
    }
}
