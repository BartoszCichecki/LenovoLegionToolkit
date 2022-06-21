using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Features
{
    public class RGBKeyboardBacklightFeature : IFeature<RGBKeyboardBacklightState>
    {
        private readonly AsyncLock _ioLock = new();

        private readonly RGBKeyboardSettings _settings;

        public RGBKeyboardBacklightFeature(RGBKeyboardSettings settings)
        {
            _settings = settings;
        }

        public async Task<RGBKeyboardBacklightState[]> GetAllStatesAsync()
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
                return new[] { _settings.Store.State };
        }

        public async Task<RGBKeyboardBacklightState> GetStateAsync()
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
                return _settings.Store.State;
        }

        public async Task SetStateAsync(RGBKeyboardBacklightState state)
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                _settings.Store.State = state;
                _settings.SynchronizeStore();
            }
        }
    }
}
