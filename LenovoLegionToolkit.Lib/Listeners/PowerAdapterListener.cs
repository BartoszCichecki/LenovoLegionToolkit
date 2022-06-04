﻿using System;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerAdapterListener : IListener<EventArgs>
    {
        public event EventHandler<EventArgs>? Changed;

        public void Start()
        {
            Stop();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        public void Stop() => SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e) => Changed?.Invoke(this, EventArgs.Empty);
    }
}
