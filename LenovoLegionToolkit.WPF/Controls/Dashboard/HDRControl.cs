using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class HDRControl : AbstractToggleFeatureCardControl<HDRState>
    {
        private readonly DisplayConfigurationListener _listener = IoCContainer.Resolve<DisplayConfigurationListener>();

        protected override HDRState OnState => HDRState.On;

        protected override HDRState OffState => HDRState.Off;

        public HDRControl()
        {
            Icon = SymbolRegular.Hdr24;
            Title = Resource.HDRControl_Title;
            Subtitle = Resource.HDRControl_Message;

            _listener.Changed += Listener_Changed;
        }

        private void Listener_Changed(object? sender, EventArgs e) => Dispatcher.Invoke(async () =>
        {
            if (!IsLoaded || !IsVisible)
                return;

            await RefreshAsync();
        });
    }
}
