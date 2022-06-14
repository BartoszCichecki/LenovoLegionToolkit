using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class ProcessListener : IListener<ProcessEventInfo>
    {
        private static readonly int _pollRate = 2;

        private static readonly string[] _ignoredNames =
        {
            "msedgewebview2.exe",
        };

        private static readonly string[] _ignoredPaths =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
        };

        public event EventHandler<ProcessEventInfo>? Changed;

        private readonly InstanceEventListener _instanceCreationListener;
        private readonly InstanceEventListener _instanceDeletionListener;

        public ProcessListener()
        {
            _instanceCreationListener = new InstanceEventListener(ProcessEventInfoType.Started, "__InstanceCreationEvent");
            _instanceCreationListener.Changed += InstanceListener_Changed;

            _instanceDeletionListener = new InstanceEventListener(ProcessEventInfoType.Stopped, "__InstanceDeletionEvent");
            _instanceDeletionListener.Changed += InstanceListener_Changed;
        }

        private void InstanceListener_Changed(object? sender, ProcessEventInfo e)
        {
            if (string.IsNullOrWhiteSpace(e.Process.Name) || _ignoredNames.Contains(e.Process.Name, StringComparer.InvariantCultureIgnoreCase))
                return;
            if (string.IsNullOrWhiteSpace(e.Process.ExecutablePath) || _ignoredPaths.Any(p => e.Process.ExecutablePath.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                return;

            Changed?.Invoke(this, e);
        }

        private class InstanceEventListener : AbstractWMIListener<ProcessEventInfo>
        {
            private readonly ProcessEventInfoType _type;

            public InstanceEventListener(ProcessEventInfoType type, string eventName)
                : base("ROOT\\CIMV2", query: $"SELECT * FROM {eventName} WITHIN {_pollRate} WHERE TargetInstance ISA 'Win32_Process'")
            {
                _type = type;
            }

            protected override ProcessEventInfo GetValue(PropertyDataCollection properties)
            {
                if (properties["TargetInstance"].Value is not ManagementBaseObject mbo)
                    return default;

                var processName = mbo.Properties["Name"].Value?.ToString() ?? string.Empty;
                var executablePath = mbo.Properties["ExecutablePath"].Value?.ToString() ?? string.Empty;

                processName = Path.GetFileNameWithoutExtension(processName);

                return new(_type, new(processName, executablePath));
            }

            protected override Task OnChangedAsync(ProcessEventInfo value) => Task.CompletedTask;
        }
    }
}
