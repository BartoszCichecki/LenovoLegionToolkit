using System;
using System.ComponentModel;
using System.ServiceProcess;
using Microsoft.Win32.TaskScheduler;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class VantageServiceNotFoundException : Exception
    {
        public VantageServiceNotFoundException(string message) : base(message) { }
    }

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

        public static VantageStatus Status
        {
            get
            {
                try
                {
                    return IsServicesEnabled() ? VantageStatus.Enabled : VantageStatus.Disabled;
                }
                catch (VantageServiceNotFoundException)
                {
                    return VantageStatus.NotFound;
                }
            }
        }

        public static void Enable()
        {
            Log.Instance.Trace($"Enabling...");

            SetScheduledTasksEnabled(true);
            SetServicesEnabled(true);

            Log.Instance.Trace($"Enabled");
        }

        public static void Disable()
        {
            Log.Instance.Trace($"Disabling...");

            SetScheduledTasksEnabled(false);
            SetServicesEnabled(false);

            Log.Instance.Trace($"Disabled");
        }

        private static void SetScheduledTasksEnabled(bool enabled)
        {
            var taskService = TaskService.Instance;
            foreach (var path in _scheduledTasksPaths)
                SetTasksInFolderEnabled(taskService, path, enabled);
        }

        private static void SetTasksInFolderEnabled(TaskService taskService, string path, bool enabled)
        {
            Log.Instance.Trace($"Setting tasks in folder {path} to {enabled}");

            var folder = taskService.GetFolder(path);
            if (folder is null)
            {
                Log.Instance.Trace($"Folder not found [path={path}]");
                return;
            }

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
                throw new VantageServiceNotFoundException(serviceName);
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
                Log.Instance.Trace($"Setting service {serviceName} to {enabled}");

                var service = new ServiceController(serviceName);

                try
                {
                    var startMode = enabled ? ServiceStartMode.Automatic : ServiceStartMode.Disabled;

                    Log.Instance.Trace($"Changing service {serviceName} start mode to {startMode}");

                    service.ChangeStartMode(startMode);

                    if (enabled)
                    {
                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            Log.Instance.Trace($"Starting service {serviceName}...");
                            service.Start();
                        }
                        else
                        {
                            Log.Instance.Trace($"Will not start service {serviceName} [status={service.Status}]");
                        }
                    }
                    else
                    {
                        if (service.CanStop)
                        {
                            Log.Instance.Trace($"Stopping service {serviceName}...");
                            service.Stop();
                        }
                        else
                        {
                            Log.Instance.Trace($"Will not stop service {serviceName} [status={service.Status}, canStop={service.CanStop}]");
                        }
                    }
                }
                finally
                {
                    service.Close();
                }
            }
            catch (InvalidOperationException ex) when ((ex.InnerException as Win32Exception)?.NativeErrorCode == 1060)
            {
                Log.Instance.Trace($"Service {serviceName} could not be set to {enabled}");

                throw new VantageServiceNotFoundException(serviceName);
            }
        }
    }
}
