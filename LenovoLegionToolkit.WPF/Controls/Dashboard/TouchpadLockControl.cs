using System.Threading.Tasks;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class TouchpadLockControl : AbstractToggleFeatureCardControl<TouchpadLockState>
    {
        private readonly DriverKeyListener _listener = IoCContainer.Resolve<DriverKeyListener>();

        protected override TouchpadLockState OnState => TouchpadLockState.On;

        protected override TouchpadLockState OffState => TouchpadLockState.Off;

        public TouchpadLockControl()
        {
            Icon = SymbolRegular.Tablet24;
            Title = Resource.TouchpadLockControl_Title;
            Subtitle = Resource.TouchpadLockControl_Message;

            _listener.Changed += Listener_Changed;
        }

        protected override async Task OnStateChange(ToggleSwitch toggle, IFeature<TouchpadLockState> feature)
        {
            await _listener.StopAsync();
            await base.OnStateChange(toggle, feature);
            await _listener.StartAsync();
        }

        private void Listener_Changed(object? sender, DriverKey e) => Dispatcher.Invoke(async () =>
        {
            if (!IsLoaded || !IsVisible)
                return;

            if (e.HasFlag(DriverKey.Fn_F10))
                await RefreshAsync();
        });
    }
}
