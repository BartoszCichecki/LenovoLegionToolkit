using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public abstract class AbstractFeatureAutomationStep<T> : IAutomationStep<T> where T : struct
    {
        private readonly IFeature<T> _feature = IoCContainer.Resolve<IFeature<T>>();

        public T State { get; }

        public AbstractFeatureAutomationStep(T state) => State = state;

        public async Task<bool> IsSupportedAsync()
        {
            try
            {
                _ = await _feature.GetStateAsync().ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task RunAsync()
        {
            await _feature.SetStateAsync(State).ConfigureAwait(false);
            MessagingCenter.Publish(State);
        }

        public Task<T[]> GetAllStatesAsync() => _feature.GetAllStatesAsync();

        public abstract IAutomationStep DeepCopy();
    }
}
