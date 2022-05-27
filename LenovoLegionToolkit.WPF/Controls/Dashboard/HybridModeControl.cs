using System.Threading.Tasks;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Utils;
using WPFUI.Common;
using WPFUI.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class HybridModeControl : AbstractToggleCardControl<HybridModeState>
    {
        protected override HybridModeState OnState => HybridModeState.On;

        protected override HybridModeState OffState => HybridModeState.Off;

        public HybridModeControl()
        {
            Icon = SymbolRegular.LeafOne24;
            Title = "Hybrid Mode";
            Subtitle = "Allow switching between integrated and discrete GPU.\nRequires restart.";
        }

        protected override async Task OnStateChange(ToggleSwitch toggle, IFeature<HybridModeState> feature)
        {
            var result = await MessageBoxHelper.ShowAsync(
                this,
                "Restart required",
                "Changing Hybrid Mode requires restart. Do you want to restart now?");

            if (result)
                await base.OnStateChange(toggle, feature);
            else
                toggle.IsChecked = !toggle.IsChecked;
        }
    }
}
