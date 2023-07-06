﻿using System;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class RefreshRateAutomationStepControl : AbstractComboBoxAutomationStepCardControl<RefreshRate>
{
    private readonly DisplayConfigurationListener _listener = IoCContainer.Resolve<DisplayConfigurationListener>();

    public RefreshRateAutomationStepControl(IAutomationStep<RefreshRate> step) : base(step)
    {
        Icon = SymbolRegular.DesktopPulse24;
        Title = Resource.RefreshRateAutomationStepControl_Title;
        Subtitle = Resource.RefreshRateAutomationStepControl_Message;

        _listener.Changed += Listener_Changed;
    }

    protected override string ComboBoxItemDisplayName(RefreshRate value)
    {
        var str = base.ComboBoxItemDisplayName(value);
        return LocalizationHelper.ForceLeftToRight(str);
    }

    private void Listener_Changed(object? sender, EventArgs e) => Dispatcher.Invoke(async () =>
    {
        if (IsLoaded)
            await RefreshAsync();
    });
}
