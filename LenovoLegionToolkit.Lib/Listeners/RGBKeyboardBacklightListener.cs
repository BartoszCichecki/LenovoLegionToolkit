using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class RGBKeyboardBacklightListener : AbstractWMIListener<EventArgs, RGBKeyboardBacklightChanged, int>
{
    private readonly RGBKeyboardBacklightController _controller;

    public RGBKeyboardBacklightListener(RGBKeyboardBacklightController controller)
        : base(WMI.LenovoGameZoneLightProfileChangeEvent.Listen)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }

    protected override RGBKeyboardBacklightChanged GetValue(int value) => default;

    protected override EventArgs GetEventArgs(RGBKeyboardBacklightChanged value) => EventArgs.Empty;

    protected override async Task OnChangedAsync(RGBKeyboardBacklightChanged value)
    {
        try
        {
            if (!await _controller.IsSupportedAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Not supported.");

                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Taking ownership...");

            await _controller.SetLightControlOwnerAsync(true).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting next preset set...");

            var preset = await _controller.SetNextPresetAsync().ConfigureAwait(false);

            if (preset == RGBKeyboardBacklightPreset.Off)
                MessagingCenter.Publish(new Notification(NotificationType.RGBKeyboardBacklightOff, preset.GetDisplayName()));
            else
                MessagingCenter.Publish(new Notification(NotificationType.RGBKeyboardBacklightChanged, preset.GetDisplayName()));

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Next preset set");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to set next keyboard backlight preset.", ex);
        }
    }
}
