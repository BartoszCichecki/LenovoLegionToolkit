using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class FnLockControl : AbstractToggleCardControl<FnLockState>
    {
        private readonly SpecialKeyListener _listener = IoCContainer.Resolve<SpecialKeyListener>();

        protected override FnLockState OnState => FnLockState.On;

        protected override FnLockState OffState => FnLockState.Off;

        public FnLockControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Fn Lock";
            Subtitle = "Use secondary functions of F1-F12 keys without holding Fn key.";

            _listener.Changed += Listener_Changed;
        }

        private void Listener_Changed(object? sender, SpecialKey e) => Dispatcher.Invoke(async () =>
        {
            if (!IsLoaded || !IsVisible)
                return;

            if (e == SpecialKey.Fn_LockOn || e == SpecialKey.Fn_LockOff)
                await RefreshAsync();
        });
    }
}
