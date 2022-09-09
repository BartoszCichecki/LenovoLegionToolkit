using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using Octokit;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class SystemThemeListener : IListener<SystemThemeSettings>
    {
        public event EventHandler<SystemThemeSettings>? Changed;

        private IDisposable? _darkModeListener, _accentColorListener;

        private RGBColor? _currentColor;

        private bool _started;

        public Task StartAsync()
        {
            if (_started)
                return Task.CompletedTask;

            _darkModeListener = SystemTheme.GetDarkModeListener(InvokeChangedEvent);
            _accentColorListener = SystemTheme.GetAccentColorListener(() =>
            {
                var color = SystemTheme.GetAccentColor();

                // Ignore alpha channel transition events
                if (_currentColor.Equals(color))
                    return;

                _currentColor = color;

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

        private void InvokeChangedEvent()
        {
            Changed?.Invoke(this, new(SystemTheme.GetDarkMode(), _currentColor ?? SystemTheme.GetAccentColor()));
        }
    }
}
