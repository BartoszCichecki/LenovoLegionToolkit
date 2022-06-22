using System.ComponentModel.DataAnnotations;

namespace LenovoLegionToolkit.Lib.Automation
{
    public enum DeactivateGPUAutomationStepState
    {
        [Display(Name = "Kill apps")]
        KillApps,
        [Display(Name = "Restart GPU")]
        RestartGPU,
    }
}
