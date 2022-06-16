using System;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class PowerModeControl : AbstractComboBoxCardControl<PowerModeState>
    {
        private readonly PowerModeListener _powerModeListener = IoCContainer.Resolve<PowerModeListener>();
        private readonly PowerPlanListener _powerPlanListener = IoCContainer.Resolve<PowerPlanListener>();

        public PowerModeControl()
        {
            Icon = SymbolRegular.Gauge24;
            Title = "Power Mode";
            Subtitle = "Select performance mode.\nYou can switch mode with Fn+Q.";

            _powerModeListener.Changed += PowerModeListener_Changed;
            _powerPlanListener.Changed += PowerPlanListener_Changed;
        }

        private void PowerModeListener_Changed(object? sender, PowerModeState e) => Dispatcher.Invoke(async () =>
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        });

        private void PowerPlanListener_Changed(object? sender, EventArgs e) => Dispatcher.Invoke(async () =>
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        });
    }
}
