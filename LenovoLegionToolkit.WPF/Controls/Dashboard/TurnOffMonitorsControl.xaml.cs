using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public partial class TurnOffMonitorsControl
{
    private readonly NativeWindowsMessageListener _nativeWindowsMessageListener = IoCContainer.Resolve<NativeWindowsMessageListener>();

    public TurnOffMonitorsControl() => InitializeComponent();

    private async void TurnOffButton_Click(object sender, RoutedEventArgs e)
    {
        _turnOffButton.IsEnabled = false;
        await _nativeWindowsMessageListener.TurnOffMonitorAsync();
        _turnOffButton.IsEnabled = true;
    }

    protected override Task OnRefreshAsync() => Task.CompletedTask;

    protected override void OnFinishedLoading() { }
}
