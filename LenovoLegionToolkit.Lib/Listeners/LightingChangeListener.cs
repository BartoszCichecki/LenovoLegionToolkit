using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Features.PanelLogo;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class LightingChangeListener : AbstractWMIListener<LightingChangeListener.ChangedEventArgs, LightingChangeState, int>
{
    public class ChangedEventArgs : EventArgs
    {
        public LightingChangeState State { get; init; }
    }

    private readonly PanelLogoBacklightFeature _panelLogoBacklightFeature;
    private readonly PortsBacklightFeature _portsBacklightFeature;
    private readonly FnKeysDisabler _fnKeysDisabler;

    public LightingChangeListener(PanelLogoBacklightFeature panelLogoBacklightFeature, PortsBacklightFeature portsBacklightFeature, FnKeysDisabler fnKeysDisabler)
        : base(WMI.LenovoLightingEvent.Listen)
    {
        _panelLogoBacklightFeature = panelLogoBacklightFeature ?? throw new ArgumentNullException(nameof(panelLogoBacklightFeature));
        _portsBacklightFeature = portsBacklightFeature ?? throw new ArgumentNullException(nameof(portsBacklightFeature));
        _fnKeysDisabler = fnKeysDisabler ?? throw new ArgumentNullException(nameof(fnKeysDisabler));
    }

    protected override LightingChangeState GetValue(int value)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received. [value={value}]");

        var result = (LightingChangeState)value;
        return result;
    }

    protected override ChangedEventArgs GetEventArgs(LightingChangeState value) => new() { State = value };

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
