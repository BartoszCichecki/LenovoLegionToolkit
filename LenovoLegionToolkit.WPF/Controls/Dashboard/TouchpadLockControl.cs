using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class TouchpadLockControl : AbstractToggleCardControl<TouchpadLockState>
    {
        protected override TouchpadLockState OnState => TouchpadLockState.On;

        protected override TouchpadLockState OffState => TouchpadLockState.Off;

        public TouchpadLockControl()
        {
            Icon = SymbolRegular.Tablet24;
            Title = "Touchpad Lock";
            Subtitle = "Disable touchpad.";
        }
    }
}
