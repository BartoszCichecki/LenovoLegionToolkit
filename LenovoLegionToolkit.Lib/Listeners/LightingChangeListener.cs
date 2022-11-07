using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class LightingChangeListener : AbstractWMIListener<LightingChangeState>
    {
        public LightingChangeListener() : base("ROOT\\WMI", "LENOVO_LIGHTING_EVENT") { }

        protected override LightingChangeState GetValue(PropertyDataCollection properties)
        {
            var property = properties["Key_ID"];
            var propertyValue = Convert.ToInt32(property.Value);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [value={propertyValue}]");

            var result = (LightingChangeState)propertyValue;
            return result;
        }

        protected override Task OnChangedAsync(LightingChangeState value) => Task.CompletedTask;
    }
}
