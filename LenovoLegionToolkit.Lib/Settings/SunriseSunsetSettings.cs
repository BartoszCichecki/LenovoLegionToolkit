using System;

namespace LenovoLegionToolkit.Lib.Settings;

public class SunriseSunsetSettings : AbstractSettings<SunriseSunsetSettings.SunriseSunsetSettingsStore>
{
    public class SunriseSunsetSettingsStore
    {
        public DateTime? LastCheckDateTime { get; set; }
        public Time? Sunrise { get; set; }
        public Time? Sunset { get; set; }
    }

    protected override string FileName => "sunrise_sunset.json";

    protected override SunriseSunsetSettingsStore Default => new();
}