using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Settings;

public class UpdateCheckSettings()
    : AbstractSettings<UpdateCheckSettings.UpdateCheckSettingsStore>("update_check.json")
{
    public class UpdateCheckSettingsStore
    {
        public DateTime? LastUpdateCheckDateTime { get; set; }
        public UpdateCheckFrequency UpdateCheckFrequency { get; set; }
    }

    protected override UpdateCheckSettingsStore Default => new()
    {
        LastUpdateCheckDateTime = null,
        UpdateCheckFrequency = UpdateCheckFrequency.PerThreeHours
    };
}
