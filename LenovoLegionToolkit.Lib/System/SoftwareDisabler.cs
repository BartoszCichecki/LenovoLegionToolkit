using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using TaskService = Microsoft.Win32.TaskScheduler.TaskService;

namespace LenovoLegionToolkit.Lib.System;

public class SoftwareDisablerException : Exception
{
    public SoftwareDisablerException(string message) : base(message) { }
}

public abstract class SoftwareDisabler
{
    protected abstract string[] ScheduledTasksPaths { get; }
    protected abstract string[] ServiceNames { get; }
    protected abstract string[] ProcessNames { get; }

    public Task<SoftwareStatus> GetStatusAsync() => Task.Run(() =>
    {
        try
        {
            return AreServicesEnabled() || AreProcessesRunning() ? SoftwareStatus.Enabled : SoftwareStatus.Disabled;
        }
        catch (SoftwareDisablerException)
        {
            return SoftwareStatus.NotFound;
        }
    });

    public virtual Task EnableAsync() => Task.Run(() =>
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Enabling...");

        SetScheduledTasksEnabled(true);
        SetServicesEnabled(true);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Enabled");
    });

    public virtual Task DisableAsync() => Task.Run(async () =>
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Disabling...");

        SetScheduledTasksEnabled(false);
        SetServicesEnabled(false);
        await KillProcessesAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Disabled");
    });

    private void SetScheduledTasksEnabled(bool enabled)
    {
        var taskService = TaskService.Instance;
        foreach (var path in ScheduledTasksPaths)
            SetTasksInFolderEnabled(taskService, path, enabled);
    }

    private void SetTasksInFolderEnabled(TaskService taskService, string path, bool enabled)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting tasks in folder {path} to {enabled}");

        var folder = taskService.GetFolder(path);
        if (folder is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Folder not found [path={path}]");

            return;
        }

        foreach (var task in folder.Tasks.ToArray())
        {
            task.Definition.Settings.Enabled = enabled;
            try
            {
                task.RegisterChanges();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to register changes on task {task.Name} in {task.Path}.", ex);

                throw new SoftwareDisablerException($"Failed to register changes on task {task.Name} in {task.Path}.");
            }
        }
    }

    private bool AreServicesEnabled()
    {
        var result = true;
        foreach (var serviceName in ServiceNames)
            result &= IsServiceEnabled(serviceName);
        return result;
    }

    private bool IsServiceEnabled(string serviceName)
    {
        try
        {
            var service = new ServiceController(serviceName);
            return service.Status != ServiceControllerStatus.Stopped || service.Status == ServiceControllerStatus.StopPending;
        }
        catch (InvalidOperationException)
        {
            throw new SoftwareDisablerException(serviceName);
        }
    }

    private void SetServicesEnabled(bool enabled)
    {
        foreach (var serviceName in ServiceNames)
            SetServiceEnabled(serviceName, enabled);
    }

    private void SetServiceEnabled(string serviceName, bool enabled)
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting service {serviceName} to {enabled}");

            var service = new ServiceController(serviceName);

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Changing service {serviceName} start mode to {enabled}");

                service.ChangeStartMode(enabled);

                if (enabled)
                {
                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Starting service {serviceName}...");
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running);
                    }
                    else
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Will not start service {serviceName} [status={service.Status}]");
                    }
                }
                else
                {
                    if (service.CanStop)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Stopping service {serviceName}...");
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    else
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Will not stop service {serviceName} [status={service.Status}, canStop={service.CanStop}]");
                    }
                }
            }
            finally
            {
                service.Close();
            }
        }
        catch (InvalidOperationException ex) when (ex.InnerException is Win32Exception { NativeErrorCode: 1060 })
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Service {serviceName} could not be set to {enabled}");

            throw new SoftwareDisablerException(serviceName);
        }
    }

    protected virtual bool AreProcessesRunning()
    {
        foreach (var process in Process.GetProcesses())
            foreach (var processName in ProcessNames)
            {
                try
                {
                    if (process.ProcessName.StartsWith(processName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
                catch
                {
                }
            }

        return false;
    }

    protected virtual async Task KillProcessesAsync()
    {
        foreach (var process in Process.GetProcesses())
            foreach (var processName in ProcessNames)
            {
                try
                {
                    if (process.ProcessName.StartsWith(processName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        process.Kill();
                        await process.WaitForExitAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Couldn't kill process.", ex);
                }
            }
    }
}