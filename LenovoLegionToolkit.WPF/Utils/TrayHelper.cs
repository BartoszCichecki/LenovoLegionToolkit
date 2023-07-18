using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Interop;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Assets;
using LenovoLegionToolkit.WPF.Controls;
using LenovoLegionToolkit.WPF.Resources;
using Windows.Win32;
using Windows.Win32.Foundation;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Brushes = System.Windows.Media.Brushes;
using MenuItem = Wpf.Ui.Controls.MenuItem;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using ToolTip = System.Windows.Controls.ToolTip;

namespace LenovoLegionToolkit.WPF.Utils;

public class TrayHelper : IDisposable
{
    private const string NAVIGATION_TAG = "navigation";
    private const string STATIC_TAG = "static";

    private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();

    private readonly NotifyIcon _notifyIcon = new()
    {
        Icon = AssetResources.icon,
        // Text = Resource.AppName
    };

    private readonly ContextMenu _contextMenu = new();

    private readonly ToolTip _toolTip = new()
    {
        Content = new StatusTrayPopup(),
        Placement = PlacementMode.Mouse,
        Background = Brushes.Transparent,
        BorderBrush = Brushes.Transparent,
        BorderThickness = new(0),
        MaxWidth = double.PositiveInfinity,
        MaxHeight = double.PositiveInfinity,
    };

    private readonly Timer _toolTipTimer = new()
    {
        Interval = 250,
    };

    private readonly Action _bringToForeground;

    private Rectangle? _mousePositionOnTooltipOpen;

    public TrayHelper(INavigation navigation, Action bringToForeground)
    {
        _bringToForeground = bringToForeground;

        InitializeStaticItems(navigation);

        _themeManager.ThemeApplied += (_, _) => _contextMenu.Resources = App.Current.Resources;

        _toolTipTimer.Tick += ToolTipTimer_Tick;
        _toolTip.Closed += ToolTip_Closed;
        _notifyIcon.MouseMove += NotifyIcon_MouseMove;
        _notifyIcon.MouseClick += NotifyIcon_MouseClick;

        _notifyIcon.Visible = true;
    }

    private void InitializeStaticItems(INavigation navigation)
    {
        foreach (var navigationItem in navigation.Items.OfType<NavigationItem>())
        {
            var navigationMenuItem = new MenuItem
            {
                SymbolIcon = navigationItem.Icon,
                Header = navigationItem.Content,
                Tag = NAVIGATION_TAG
            };
            navigationMenuItem.Click += async (_, _) =>
            {
                _contextMenu.IsOpen = false;
                _bringToForeground();

                await Task.Delay(TimeSpan.FromMilliseconds(500));
                navigation.Navigate(navigationItem.PageTag);
            };
            _contextMenu.Items.Add(navigationMenuItem);
        }

        _contextMenu.Items.Add(new Separator { Tag = NAVIGATION_TAG });

        var openMenuItem = new MenuItem { Header = Resource.Open, Tag = STATIC_TAG };
        openMenuItem.Click += (_, _) =>
        {
            _contextMenu.IsOpen = false;
            _bringToForeground();
        };
        _contextMenu.Items.Add(openMenuItem);

        var closeMenuItem = new MenuItem { Header = Resource.Close, Tag = STATIC_TAG };
        closeMenuItem.Click += async (_, _) =>
        {
            _contextMenu.IsOpen = false;
            await App.Current.ShutdownAsync();
        };
        _contextMenu.Items.Add(closeMenuItem);
    }

    private void ToolTipTimer_Tick(object? sender, EventArgs e)
    {
        if (!_mousePositionOnTooltipOpen.HasValue)
            return;

        var point = Cursor.Position;
        if (_mousePositionOnTooltipOpen.Value.Contains(point))
            return;

        _toolTip.IsOpen = false;
    }

    private void ToolTip_Closed(object sender, RoutedEventArgs e)
    {
        _toolTipTimer.Enabled = false;
        _mousePositionOnTooltipOpen = null;
    }

    private void NotifyIcon_MouseMove(object? sender, MouseEventArgs e)
    {
        if (_toolTip.IsOpen)
            return;

        var size = _notifyIcon.Icon.Size.Width;
        var point = Cursor.Position;
        _mousePositionOnTooltipOpen = new(point.X - size / 2, point.Y - size / 2, size, size);

        _toolTip.IsOpen = true;
        _toolTipTimer.Enabled = true;

        if (PresentationSource.FromVisual(_toolTip) is HwndSource source && source.Handle != IntPtr.Zero)
            PInvoke.SetForegroundWindow(new HWND(source.Handle));
    }

    private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                {
                    _bringToForeground();

                    break;
                }
            case MouseButtons.Right:
                {
                    _contextMenu.IsOpen = true;

                    if (PresentationSource.FromVisual(_contextMenu) is HwndSource source && source.Handle != IntPtr.Zero)
                        PInvoke.SetForegroundWindow(new HWND(source.Handle));

                    break;
                }
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _notifyIcon.Dispose();
    }
}
