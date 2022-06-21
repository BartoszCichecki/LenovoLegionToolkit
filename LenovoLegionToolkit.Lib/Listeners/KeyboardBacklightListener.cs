using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class KeyboardBacklightListener : AbstractWMIListener<KeyboardBacklight>
    {
        public KeyboardBacklightListener() : base("ROOT\\WMI", "LENOVO_GAMEZONE_LIGHT_PROFILE_CHANGE_EVENT") { }

        protected override KeyboardBacklight GetValue(PropertyDataCollection properties) => default;

        protected override Task OnChangedAsync(KeyboardBacklight value) => Task.CompletedTask;
    }
}
