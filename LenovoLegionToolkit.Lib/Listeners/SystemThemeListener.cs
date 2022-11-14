using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class SystemThemeListener : IListener<EventArgs>
    {
        public event EventHandler<EventArgs>? Changed;

        private IDisposable? _darkModeListener, _accentColorListener;

        private RGBColor? _currentRegColor;

        private bool _started;

        public Task StartAsync()
        {
            if (_started)
                return Task.CompletedTask;

            _darkModeListener = SystemTheme.GetDarkModeListener(InvokeChangedEvent);
            _accentColorListener = SystemTheme.GetAccentColorListener(() =>
            {
                var color = SystemTheme.GetAccentColorReg();

                // Ignore alpha channel transition events
                if (_currentRegColor.Equals(color))
                    return;

                _currentRegColor = color;

                InvokeChangedEvent();
            });

            _started = true;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _darkModeListener?.Dispose();
            _accentColorListener?.Dispose();

            _started = false;

            return Task.CompletedTask;
        }

        private void InvokeChangedEvent() => Changed?.Invoke(this, EventArgs.Empty);
    }
}
