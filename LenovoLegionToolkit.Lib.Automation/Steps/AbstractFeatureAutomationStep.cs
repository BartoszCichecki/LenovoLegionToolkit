using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using PubSub;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public abstract class AbstractFeatureAutomationStep<T> : IAutomationStep<T> where T : struct
    {
        private readonly IFeature<T> _feature = IoCContainer.Resolve<IFeature<T>>();

        public T State { get; }

        public AbstractFeatureAutomationStep(T state) => State = state;

        public async Task RunAsync()
        {
            var currentState = await _feature.GetStateAsync().ConfigureAwait(false);
            if (State.Equals(currentState))
                return;
            await _feature.SetStateAsync(State).ConfigureAwait(false);

            Hub.Default.Publish(State);
        }

        public Task<T[]> GetAllStatesAsync() => _feature.GetAllStatesAsync();

        public abstract IAutomationStep DeepCopy();
    }
}
