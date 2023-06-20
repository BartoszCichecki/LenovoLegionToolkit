using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public interface IAutomationStep
{
    Task<bool> IsSupportedAsync();

    Task RunAsync(AutomationEnvironment environment);

    IAutomationStep DeepCopy();
}

public interface IAutomationStep<T> : IAutomationStep where T : struct
{
    T State { get; }

    Task<T[]> GetAllStatesAsync();
}
