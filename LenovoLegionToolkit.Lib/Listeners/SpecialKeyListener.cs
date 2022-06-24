using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class SpecialKeyListener : AbstractWMIListener<SpecialKey>
    {
        private readonly RefreshRateFeature _feature;

        public SpecialKeyListener(RefreshRateFeature feature) : base("ROOT\\WMI", "LENOVO_UTILITY_EVENT")
        {
            _feature = feature;
        }

        protected override SpecialKey GetValue(PropertyDataCollection properties)
        {
            var property = properties["PressTypeDataVal"];
            var propertyValue = Convert.ToInt32(property.Value);
            var value = (SpecialKey)(object)propertyValue;
            return value;
        }

        protected override Task OnChangedAsync(SpecialKey value)
        {
            if (value == SpecialKey.Fn_R)
                return ToggleRefreshRateAsync();

            return Task.CompletedTask;
        }

        private async Task ToggleRefreshRateAsync()
        {
            try
            {
                if (await FnKeys.GetStatusAsync() == FnKeysStatus.Enabled)
                    return;

                var all = await _feature.GetAllStatesAsync().ConfigureAwait(false);
                var current = await _feature.GetStateAsync().ConfigureAwait(false);

                if (all is null || all.Length < 2)
                    return;

                var currentIndex = Array.IndexOf(all, current);
                var newIndex = currentIndex + 1;

                if (newIndex >= all.Length)
                    newIndex = 0;

                current = all[newIndex];

                await _feature.SetStateAsync(current).ConfigureAwait(false);
            }
            catch { }
        }
    }
}
