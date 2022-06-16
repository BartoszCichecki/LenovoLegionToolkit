using System;
using System.Diagnostics;
using System.Security.Principal;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.TaskScheduler;

namespace LenovoLegionToolkit.Lib.System
{
    public static class Autorun
    {
        private const string TaskName = "LenovoLegionToolkit_Autorun_6efcc882-924c-4cbc-8fec-f45c25696f98";

        public static bool IsEnabled => TaskService.Instance.GetTask(TaskName) is not null;

        public static void Validate()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Validating autorun...");

            var currentTask = TaskService.Instance.GetTask(TaskName);
            if (currentTask is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Autorun is not enabled.");
                return;
            }

            var mainModule = Process.GetCurrentProcess().MainModule;
            if (mainModule is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Main module is null.");
                return;
            }

            var fileVersion = mainModule.FileVersionInfo.FileVersion;
            if (fileVersion is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"File version is null.");
                return;
            }

            if (currentTask.Definition.Data == fileVersion)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Autorun settings seems to be fine.");
                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Enabling autorun again...");

            Enable();
        }

        public static void Enable()
        {
            Disable();

            var mainModule = Process.GetCurrentProcess().MainModule;
            if (mainModule is null)
                throw new InvalidOperationException("Main Module cannot be null");

            var filename = mainModule.FileName;
            if (filename is null)
                throw new InvalidOperationException("Current process file name cannot be null");

            var fileVersion = mainModule.FileVersionInfo.FileVersion;
            if (fileVersion is null)
                throw new InvalidOperationException("Current process file version cannot be null");

            var currentUser = WindowsIdentity.GetCurrent().Name;

            var ts = TaskService.Instance;
            var td = ts.NewTask();
            td.Data = fileVersion;
            td.Principal.UserId = currentUser;
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Triggers.Add(new LogonTrigger { UserId = currentUser, Delay = new TimeSpan(0, 1, 0) });
            td.Actions.Add($"\"{filename}\"", "--minimized");
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.StopIfGoingOnBatteries = false;
            ts.RootFolder.RegisterTaskDefinition(TaskName, td);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Autorun enabled");
        }

        public static void Disable()
        {
            try
            {
                TaskService.Instance.RootFolder.DeleteTask(TaskName);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Autorun disabled");
            }
            catch
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Autorun was not enabled");
            }
        }
    }
}
