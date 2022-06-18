using System;

namespace LenovoLegionToolkit.Lib.System
{
    public static class Battery
    {
        public static double? GetBatteryTemperatureC()
        {
            uint inBuffer = 0;
            var result = Native.DeviceIoControl(Drivers.GetEnergy(),
                                                0x83102138,
                                                ref inBuffer,
                                                4,
                                                out BatteryInformation bi,
                                                0x53,
                                                out _,
                                                IntPtr.Zero);
            if (!result)
                return null;

            var temperature = bi.Temperature;
            var temperatureC = (temperature - 2731.6) / 10;
            return temperatureC;
        }
    }
}
