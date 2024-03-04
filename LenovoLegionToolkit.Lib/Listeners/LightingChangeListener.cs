﻿using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Features.PanelLogo;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class LightingChangeListener(
    PanelLogoBacklightFeature panelLogoBacklightFeature,
    PortsBacklightFeature portsBacklightFeature,
    FnKeysDisabler fnKeysDisabler)
    : AbstractWMIListener<LightingChangeState, int>(WMI.LenovoLightingEvent.Listen)
{
    private readonly PanelLogoBacklightFeature _panelLogoBacklightFeature = panelLogoBacklightFeature ?? throw new ArgumentNullException(nameof(panelLogoBacklightFeature));
    private readonly PortsBacklightFeature _portsBacklightFeature = portsBacklightFeature ?? throw new ArgumentNullException(nameof(portsBacklightFeature));
    private readonly FnKeysDisabler _fnKeysDisabler = fnKeysDisabler ?? throw new ArgumentNullException(nameof(fnKeysDisabler));

    protected override LightingChangeState GetValue(int value)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received. [value={value}]");

        var result = (LightingChangeState)value;
        return result;
    }

    protected override async Task OnChangedAsync(LightingChangeState value)
    {
        try
        {
            if (await _fnKeysDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                return;
            }

            switch (value)
            {
                case LightingChangeState.Panel when await _panelLogoBacklightFeature.IsSupportedAsync().ConfigureAwait(false):
                    {
                        var type = await _panelLogoBacklightFeature.GetStateAsync().ConfigureAwait(false) == PanelLogoBacklightState.On
                            ? NotificationType.PanelLogoLightingOn
                            : NotificationType.PanelLogoLightingOff;

                        MessagingCenter.Publish(new Notification(type));
                        break;
                    }
                case LightingChangeState.Ports when await _portsBacklightFeature.IsSupportedAsync().ConfigureAwait(false):
                    {
                        var type = await _portsBacklightFeature.GetStateAsync().ConfigureAwait(false) == PortsBacklightState.On
                            ? NotificationType.PortLightingOn
                            : NotificationType.PortLightingOff;

                        MessagingCenter.Publish(new Notification(type));
                        break;
                    }
            }
        }
        catch { /* Ignored. */ }
    }
}
