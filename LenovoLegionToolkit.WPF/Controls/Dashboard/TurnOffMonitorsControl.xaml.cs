using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public partial class TurnOffMonitorsControl
{
    private readonly NativeWindowsMessageListener _nativeWindowsMessageListener = IoCContainer.Resolve<NativeWindowsMessageListener>();

    public TurnOffMonitorsControl() => InitializeComponent();

    private void TurnOffButton_Click(object sender, RoutedEventArgs e)
    {
        _nativeWindowsMessageListener.TurnOffMonitor();
    }

    protected override Task OnRefreshAsync() => Task.CompletedTask;

    protected override void OnFinishedLoading() { }
}
