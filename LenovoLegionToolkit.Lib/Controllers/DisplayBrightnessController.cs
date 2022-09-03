using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class DisplayBrightnessController
    {
        public Task<int> Get()
        {
            using var mclass = new ManagementClass("WmiMonitorBrightness")
            {
                Scope = new ManagementScope(@"\\.\root\wmi")
            };
            using var instances = mclass.GetInstances();
            foreach (ManagementObject instance in instances)
            {
                return Task.FromResult((int)(byte)instance.GetPropertyValue("CurrentBrightness"));
            }
            return Task.FromResult(0);
        }

        public Task Set(int brightness)
        {
            using var mclass = new ManagementClass("WmiMonitorBrightnessMethods")
            {
                Scope = new ManagementScope(@"\\.\root\wmi")
            };
            using var instances = mclass.GetInstances();
            var args = new object[] { 1, brightness };
            foreach (ManagementObject instance in instances)
            {
                instance.InvokeMethod("WmiSetBrightness", args);
            }
            return Task.CompletedTask;
        }
    }
}
