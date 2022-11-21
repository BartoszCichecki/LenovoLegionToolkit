using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class FnLockControl : AbstractToggleFeatureCardControl<FnLockState>
    {
        private readonly SpecialKeyListener _listener = IoCContainer.Resolve<SpecialKeyListener>();

        protected override FnLockState OnState => FnLockState.On;

        protected override FnLockState OffState => FnLockState.Off;

        public FnLockControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = Resource.FnLockControl_Title;
            Subtitle = Resource.FnLockControl_Message;

            _listener.Changed += Listener_Changed;
        }

        private void Listener_Changed(object? sender, SpecialKey e) => Dispatcher.Invoke(async () =>
        {
            if (!IsLoaded || !IsVisible)
                return;

            if (e is SpecialKey.FnLockOn or SpecialKey.FnLockOff)
                await RefreshAsync();
        });
    }
}
