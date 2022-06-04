using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Listeners;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class RefreshRateAutomationStepControl : AbstractComboBoxAutomationStepCardControl<RefreshRate>
    {
        private readonly DisplayConfigurationListener _listener = IoCContainer.Resolve<DisplayConfigurationListener>();

        public RefreshRateAutomationStepControl(IAutomationStep<RefreshRate> step) : base(step)
        {
            Icon = SymbolRegular.Laptop24;
            Title = "Refresh rate";
            Subtitle = "Change refresh rate of the built-in display.\n\nWARNING: This action will not run correctly, if\ninternal display is off.";

            _listener.Changed += Listener_Changed;
        }

        private void Listener_Changed(object? sender, System.EventArgs e) => Dispatcher.Invoke(async () =>
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        });
    }
}
