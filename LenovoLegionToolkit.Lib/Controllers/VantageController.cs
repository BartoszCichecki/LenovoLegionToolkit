using System;
using System.ComponentModel;
using System.ServiceProcess;
using Microsoft.Win32.TaskScheduler;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class VantageServiceNotFoundException : Exception { }

    public static class VantageController
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
            "ImControllerServicae",
            "LenovoVantageService",
        };

        public static bool IsEnabled() => IsServicesEnabled();

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

        private static bool IsServicesEnabled()
        {
            var result = true;
            foreach (var serviceName in _serviceNames)
                result &= IsServiceEnabled(serviceName);
            return result;
        }

        private static bool IsServiceEnabled(string serviceName)
        {
            try
            {
                var service = new ServiceController(serviceName);
                return service.Status != ServiceControllerStatus.Stopped || service.Status == ServiceControllerStatus.StopPending;
            }
            catch (InvalidOperationException ex) when ((ex.InnerException as Win32Exception)?.NativeErrorCode == 1060)
            {
                throw new VantageServiceNotFoundException();
            }
        }

        private static void SetServicesEnabled(bool enabled)
        {
            foreach (var serviceName in _serviceNames)
                SetServiceEnabled(serviceName, enabled);
        }

        private static void SetServiceEnabled(string serviceName, bool enabled)
        {
            try
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
            catch (InvalidOperationException ex) when ((ex.InnerException as Win32Exception)?.NativeErrorCode == 1060)
            {
                throw new VantageServiceNotFoundException();
            }
        }
    }
}
