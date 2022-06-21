using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
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
            await CheckVantageStatus().ConfigureAwait(false);

            using (await _ioLock.LockAsync().ConfigureAwait(false))
                return _settings.Store.State;
        }

        public async Task SetStateAsync(RGBKeyboardBacklightState state)
        {
            await CheckVantageStatus().ConfigureAwait(false);

            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                _settings.Store.State = state;
                _settings.SynchronizeStore();
            }
        }

        private static async Task CheckVantageStatus()
        {
            var vantageStatus = await Vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == VantageStatus.Enabled)
                throw new InvalidOperationException("Can't manage RGB keyboard with Vantage enabled.");
        }
    }
}
