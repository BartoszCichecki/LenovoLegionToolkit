using System;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Utils;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class PowerModeControl : AbstractComboBoxCardControl<PowerModeState>
    {
        private readonly PowerModeListener _powerModeListener = Container.Resolve<PowerModeListener>();
        private readonly PowerPlanListener _powerPlanListener = Container.Resolve<PowerPlanListener>();

        public PowerModeControl()
        {
            Icon = SymbolRegular.Gauge24;
            Title = "Power Mode";
            Subtitle = "Choose the mode you want to use.\nYou can switch mode using shortcut Fn+Q.";

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
