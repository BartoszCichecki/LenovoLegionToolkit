using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public abstract class AbstractFeatureAutomationStep<T> : IAutomationStep<T> where T : struct
{
    private readonly IFeature<T> _feature = IoCContainer.Resolve<IFeature<T>>();

    public T State { get; }

    protected AbstractFeatureAutomationStep(T state) => State = state;

    public Task<bool> IsSupportedAsync() => _feature.IsSupportedAsync();

    public virtual async Task RunAsync()
    {
        var currentState = await _feature.GetStateAsync().ConfigureAwait(false);
        if (!State.Equals(currentState))
            await _feature.SetStateAsync(State).ConfigureAwait(false);
        MessagingCenter.Publish(State);
    }

    public Task<T[]> GetAllStatesAsync() => _feature.GetAllStatesAsync();

    public abstract IAutomationStep DeepCopy();
}
