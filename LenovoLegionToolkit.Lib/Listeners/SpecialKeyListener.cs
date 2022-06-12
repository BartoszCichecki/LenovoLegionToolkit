using System;
using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class SpecialKeyListener : AbstractWMIListener<SpecialKey>
    {
        public SpecialKeyListener() : base("ROOT\\WMI", "LENOVO_UTILITY_EVENT") { }
        protected override SpecialKey GetValue(PropertyDataCollection properties)
        {
            var property = properties["PressTypeDataVal"];
            var propertyValue = Convert.ToInt32(property.Value);
            var value = (SpecialKey)(object)(propertyValue);
            return value;
        }

        protected override Task OnChangedAsync(SpecialKey value)
        {
            return Task.CompletedTask;
        }
    }
}
