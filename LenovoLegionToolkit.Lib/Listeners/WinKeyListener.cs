using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Listeners;

public class WinKeyListener()
    : AbstractWMIListener<EventArgs, WinKeyChanged, int>(WMI.LenovoGameZoneKeyLockStatusEvent.Listen)
{
    protected override WinKeyChanged GetValue(int value) => default;

    protected override EventArgs GetEventArgs(WinKeyChanged value) => EventArgs.Empty;

    protected override Task OnChangedAsync(WinKeyChanged value) => Task.CompletedTask;
}
