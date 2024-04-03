using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Listeners;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class TurnOffMonitorsAutomationStep : IAutomationStep
{
    private readonly NativeWindowsMessageListener _nativeWindowsMessageListener = IoCContainer.Resolve<NativeWindowsMessageListener>();

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token) => _nativeWindowsMessageListener.TurnOffMonitorAsync();

    public IAutomationStep DeepCopy() => new TurnOffMonitorsAutomationStep();
}
