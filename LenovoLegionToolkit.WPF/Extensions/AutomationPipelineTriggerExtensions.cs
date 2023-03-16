using System;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Extensions;

public static class AutomationPipelineTriggerExtensions
{
    public static SymbolRegular Icon(this IAutomationPipelineTrigger trigger) => trigger switch
    {
        IPowerStateAutomationPipelineTrigger => SymbolRegular.BatteryCharge24,
        IPowerModeAutomationPipelineTrigger => SymbolRegular.Gauge24,
        IGameAutomationPipelineTrigger => SymbolRegular.XboxController24,
        IProcessesAutomationPipelineTrigger => SymbolRegular.WindowConsole20,
        ITimeAutomationPipelineTrigger => SymbolRegular.HourglassHalf24,
        INativeWindowsMessagePipelineTrigger => SymbolRegular.Desktop24,
        IOnStartupAutomationPipelineTrigger => SymbolRegular.Flash24,
        _ => throw new ArgumentException(nameof(trigger))
    };
}
