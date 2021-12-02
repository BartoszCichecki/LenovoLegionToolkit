using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Xml.Linq;
using System.Linq;

namespace LenovoLegionToolkit.Lib
{

    public struct NVidiaInformation
    {
        public int ProcessCount { get; private set; }
        public IEnumerable<string> ProcessNames { get; private set; }

        public static NVidiaInformation Create(string xml)
        {
            var xdoc = XDocument.Parse(xml);
            var gpu = xdoc.Element("nvidia_smi_log").Element("gpu");
            var processInfo = gpu.Element("processes").Elements("process_info");
            var processesCount = processInfo.Count();
            var processNames = processInfo.Select(e => e.Element("process_name").Value).Select(Path.GetFileName);
            return new NVidiaInformation
            {
                ProcessCount = processesCount,
                ProcessNames = processNames,
            };
        }
    }

    public enum AlwaysOnUsbState
    {
        Off,
        OnWhenSleeping,
        OnAlways
    }

    public enum BatteryState
    {
        Conservation,
        Normal,
        RapidCharge
    }

    public enum FlipToStartState
    {
        Off,
        On
    }

    public enum FnLockState
    {
        Off,
        On
    }

    public enum HybridModeState
    {
        On,
        Off
    }

    public enum OverDriveState
    {
        Off,
        On
    }

    public enum PowerModeState
    {
        Quiet,
        Balance,
        Performance
    }

    public enum TouchpadLockState
    {
        Off,
        On
    }
}
