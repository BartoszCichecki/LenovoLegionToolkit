using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class WinKeyListener : AbstractWMIListener<WinKeyChanged>
    {
        public WinKeyListener() : base("ROOT\\WMI", "LENOVO_GAMEZONE_KEYLOCK_STATUS_EVENT") { }

        protected override WinKeyChanged GetValue(PropertyDataCollection properties) => default;

        protected override Task OnChangedAsync(WinKeyChanged value) => Task.CompletedTask;
    }
}
