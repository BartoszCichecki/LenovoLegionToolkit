using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public abstract class AbstractFeatureAutomationStep<T> : IAutomationStep<T> where T : struct
    {
        protected readonly IFeature<T> Feature = IoCContainer.Resolve<IFeature<T>>();

        public T State { get; }

        public AbstractFeatureAutomationStep(T state) => State = state;

        public async Task<bool> IsSupportedAsync()
        {
            try
            {
                _ = await Feature.GetStateAsync().ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task RunAsync()
        {
            var currentState = await Feature.GetStateAsync().ConfigureAwait(false);
            if (State.Equals(currentState))
                return;
            await Feature.SetStateAsync(State).ConfigureAwait(false);

            MessagingCenter.Publish(State);
        }

        public Task<T[]> GetAllStatesAsync() => Feature.GetAllStatesAsync();

        public abstract IAutomationStep DeepCopy();
    }
}
