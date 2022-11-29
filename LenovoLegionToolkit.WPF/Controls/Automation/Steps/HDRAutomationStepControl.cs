using System;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class HDRAutomationStepControl : AbstractComboBoxAutomationStepCardControl<HDRState>
{
    private readonly DisplayConfigurationListener _listener = IoCContainer.Resolve<DisplayConfigurationListener>();

    public HDRAutomationStepControl(IAutomationStep<HDRState> step) : base(step)
    {
        Icon = SymbolRegular.Hdr24;
        Title = Resource.HDRAutomationStepControl_Title;
        Subtitle = Resource.HDRAutomationStepControl_Message;

        _listener.Changed += Listener_Changed;
    }

    private void Listener_Changed(object? sender, EventArgs e) => Dispatcher.Invoke(async () =>
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    });
}