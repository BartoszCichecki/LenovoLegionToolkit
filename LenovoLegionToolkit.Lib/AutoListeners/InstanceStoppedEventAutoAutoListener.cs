using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class InstanceStoppedEventAutoAutoListener : AbstractAutoListener<(int, string)>
{
    private IDisposable? _disposable;

    protected override Task StartAsync()
    {
        _disposable = WMI.Win32.ProcessStopTrace.Listen(Handle);
        return Task.CompletedTask;
    }


    protected override Task StopAsync()
    {
        _disposable?.Dispose();
        _disposable = null;
        return Task.CompletedTask;
    }

    private void Handle(int processId, string processName) => RaiseChanged((processId, processName));
}
