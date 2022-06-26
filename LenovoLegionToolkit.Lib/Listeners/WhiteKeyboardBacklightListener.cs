using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class WhiteKeyboardBacklightListener : AbstractWMIListener<WhiteKeyboardBacklightChanged>
    {
        public WhiteKeyboardBacklightListener() : base("ROOT\\WMI", "LENOVO_LIGHTING_EVENT") { }

        protected override WhiteKeyboardBacklightChanged GetValue(PropertyDataCollection properties) => default;

        protected override Task OnChangedAsync(WhiteKeyboardBacklightChanged value) => Task.CompletedTask;
    }
}
