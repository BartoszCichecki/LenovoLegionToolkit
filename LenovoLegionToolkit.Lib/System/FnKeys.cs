using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System
{
    public class FnKeysServiceNotFoundException : Exception
    {
        public FnKeysServiceNotFoundException(string message) : base(message) { }
    }

    public static class FnKeys
    {
        private static readonly string[] _serviceNames = new[]
        {
            "LenovoFnAndFunctionKeys",
        };

        public static Task<FnKeysStatus> GetStatusAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    return IsServicesEnabled() ? FnKeysStatus.Enabled : FnKeysStatus.Disabled;
                }
                catch (FnKeysServiceNotFoundException)
                {
                    return FnKeysStatus.NotFound;
                }
            });
        }

        public static Task EnableAsync()
        {
            return Task.Run(() =>
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Enabling...");

                SetServicesEnabled(true);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Enabled");
            });
        }

        public static Task DisableAsync()
        {
            return Task.Run(() =>
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Disabling...");

                SetServicesEnabled(false);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Disabled");
            });
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
                throw new FnKeysServiceNotFoundException(serviceName);
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
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Setting service {serviceName} to {enabled}");

                var service = new ServiceController(serviceName);

                try
                {
                    var startMode = enabled ? ServiceStartMode.Automatic : ServiceStartMode.Disabled;

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Changing service {serviceName} start mode to {startMode}");

                    service.ChangeStartMode(startMode);

                    if (enabled)
                    {
                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            if (Log.Instance.IsTraceEnabled)
                                Log.Instance.Trace($"Starting service {serviceName}...");
                            service.Start();
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
            catch (InvalidOperationException ex) when ((ex.InnerException as Win32Exception)?.NativeErrorCode == 1060)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Service {serviceName} could not be set to {enabled}");

                throw new FnKeysServiceNotFoundException(serviceName);
            }
        }
    }
}
