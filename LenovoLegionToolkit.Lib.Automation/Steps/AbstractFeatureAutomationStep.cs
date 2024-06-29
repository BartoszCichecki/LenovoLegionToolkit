using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Messaging;
using LenovoLegionToolkit.Lib.Messaging.Messages;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public abstract class AbstractFeatureAutomationStep<T>(T state)
    : IAutomationStep<T> where T : struct
{
    private readonly IFeature<T> _feature = IoCContainer.Resolve<IFeature<T>>();

    public T State { get; } = state;

    public Task<bool> IsSupportedAsync() => _feature.IsSupportedAsync();

    public virtual async Task RunAsync(AutomationContext context, AutomationEnvironment environment,
        CancellationToken token)
    {
        var currentState = await _feature.GetStateAsync().ConfigureAwait(false);
        if (!State.Equals(currentState))
            await _feature.SetStateAsync(State).ConfigureAwait(false);
        MessagingCenter.Publish(new FeatureStateMessage<T>(State));
    }

    public Task<T[]> GetAllStatesAsync() => _feature.GetAllStatesAsync();

    public abstract IAutomationStep DeepCopy();
}
