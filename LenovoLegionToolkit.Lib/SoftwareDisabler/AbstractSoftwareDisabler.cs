using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using TaskService = Microsoft.Win32.TaskScheduler.TaskService;

namespace LenovoLegionToolkit.Lib.SoftwareDisabler;

public class SoftwareDisablerException : Exception
{
    public SoftwareDisablerException(string message) : base(message) { }
}

public abstract class AbstractSoftwareDisabler
{
    protected abstract string[] ScheduledTasksPaths { get; }
    protected abstract string[] ServiceNames { get; }
    protected abstract string[] ProcessNames { get; }

    public Task<SoftwareStatus> GetStatusAsync() => Task.Run(() =>
    {
        SoftwareStatus status;

        try
        {
            var areServicesEnabled = AreServicesEnabled();
            var areProcessesRunning = AreProcessesRunning();
            status = areServicesEnabled || areProcessesRunning ? SoftwareStatus.Enabled : SoftwareStatus.Disabled;
        }
        catch (SoftwareDisablerException)
        {
            status = SoftwareStatus.NotFound;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Status: {status} [type={GetType().Name}]");

        return status;
    });

    public virtual Task EnableAsync() => Task.Run(() =>
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Enabling... [type={GetType().Name}]");

        SetScheduledTasksEnabled(true);
        SetServicesEnabled(true);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Enabled [type={GetType().Name}]");
    });

    public virtual Task DisableAsync() => Task.Run(async () =>
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Disabling... [type={GetType().Name}]");

        SetScheduledTasksEnabled(false);
        SetServicesEnabled(false);
        await KillProcessesAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Disabled [type={GetType().Name}]");
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
            Log.Instance.Trace($"Setting tasks in folder {path} to {enabled}. [type={GetType().Name}]");

        var folder = taskService.GetFolder(path);
        if (folder is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Folder not found [path={path}, type={GetType().Name}]]");

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

                throw new SoftwareDisablerException($"Failed to register changes on task {task.Name} in {task.Path}. [type={GetType().Name}]");
            }
        }
    }

    private bool AreServicesEnabled() => ServiceNames.Aggregate(false, (current, serviceName) => current | IsServiceEnabled(serviceName));

    private static bool IsServiceEnabled(string serviceName)
    {
        try
        {
            if (!ServiceController.GetServices().Any(s => s.ServiceName == serviceName))
                return false;

            var service = new ServiceController(serviceName);
            return service.Status is not ServiceControllerStatus.Stopped;
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
                Log.Instance.Trace($"Setting service {serviceName} to {enabled}. [type={GetType().Name}]");

            var service = new ServiceController(serviceName);

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Changing service {serviceName} start mode to {enabled}.  [type={GetType().Name}]");

                service.ChangeStartMode(enabled);

                if (enabled)
                {
                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Starting service {serviceName}... [type={GetType().Name}]");
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running);
                    }
                    else
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Will not start service {serviceName}. [status={service.Status}, type={GetType().Name}]]");
                    }
                }
                else
                {
                    if (service.CanStop)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Stopping service {serviceName}... [type={GetType().Name}]");
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    else
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Will not stop service {serviceName}. [status={service.Status}, canStop={service.CanStop}, type={GetType().Name}]]");
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
