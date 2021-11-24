using System.ServiceProcess;
using Microsoft.Win32.TaskScheduler;

namespace LenovoLegionToolkit.Lib.Utils
{
    public static class Vantage
    {
        private static readonly string[] _scheduledTasksPaths = new[]
        {
            "Lenovo\\BatteryGauge",
            "Lenovo\\ImController",
            "Lenovo\\ImController\\Plugins",
            "Lenovo\\ImController\\TimeBasedEvents",
            "Lenovo\\Vantage",
            "Lenovo\\Vantage\\Schedule",
        };

        private static readonly string[] _serviceNames = new[]
        {
            "ImControllerService",
            "LenovoVantageService",
        };

        public static void Enable()
        {
            SetScheduledTasksEnabled(true);
            SetServicesEnabled(true);
        }

        public static void Disable()
        {
            SetScheduledTasksEnabled(false);
            SetServicesEnabled(false);
        }

        private static void SetScheduledTasksEnabled(bool enabled)
        {
            var taskService = TaskService.Instance;
            foreach (var path in _scheduledTasksPaths)
                SetTasksInFolderEnabled(taskService, path, enabled);
        }

        private static void SetTasksInFolderEnabled(TaskService taskService, string path, bool enabled)
        {
            var folder = taskService.GetFolder(path);
            if (folder is null)
                return;

            foreach (var task in folder.Tasks)
            {
                task.Definition.Settings.Enabled = enabled;
                task.RegisterChanges();
            }
        }

        private static void SetServicesEnabled(bool enabled)
        {
            foreach (var serviceName in _serviceNames)
                SetServiceEnabled(serviceName, enabled);
        }

        private static void SetServiceEnabled(string serviceName, bool enabled)
        {
            var service = new ServiceController(serviceName);

            try
            {
                var startMode = enabled ? ServiceStartMode.Automatic : ServiceStartMode.Disabled;
                service.ChangeStartMode(startMode);

                if (enabled)
                {
                    if (service.Status != ServiceControllerStatus.Running)
                        service.Start();
                }
                else
                {
                    if (service.CanStop)
                        service.Stop();
                }
            }
            finally
            {
                service.Close();
            }
        }
    }
}
