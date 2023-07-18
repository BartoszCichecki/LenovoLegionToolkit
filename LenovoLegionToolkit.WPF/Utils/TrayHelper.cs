using System.Windows.Forms;
using LenovoLegionToolkit.WPF.Assets;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Utils;

public class TrayHelper
{
    private readonly NotifyIcon _notifyIcon;

    public TrayHelper()
    {
        _notifyIcon = new NotifyIcon
        {
            Text = Resource.AppName,
            Icon = AssetResources.icon,
            Visible = true
        };
    }
}
