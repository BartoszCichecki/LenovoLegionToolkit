using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class RGBKeyboardBacklightController
    {
        private readonly AsyncLock _ioLock = new();

        private readonly RGBKeyboardSettings _settings;

        public RGBKeyboardBacklightController(RGBKeyboardSettings settings) => _settings = settings;

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
