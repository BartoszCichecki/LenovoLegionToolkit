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

    public SunriseSunsetSettings() : base("sunrise_sunset.json") { }
}