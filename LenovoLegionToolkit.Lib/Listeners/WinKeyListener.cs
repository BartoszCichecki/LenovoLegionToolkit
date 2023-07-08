using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Listeners;

public class WinKeyListener : AbstractWMIListener<WinKeyChanged, int>
{
    public WinKeyListener() : base(WMI.LenovoGameZoneKeyLockStatusEvent.Listen) { }

    protected override WinKeyChanged GetValue(int value) => default;

    protected override Task OnChangedAsync(WinKeyChanged value) => Task.CompletedTask;
}
