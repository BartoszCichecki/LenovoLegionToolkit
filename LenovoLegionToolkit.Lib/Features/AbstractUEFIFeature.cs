using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Vanara.PInvoke;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractUEFIFeature<T> : IFeature<T> where T : struct, Enum, IComparable
    {
        private readonly string _guid;
        private readonly string _scopeName;
        private readonly Kernel32.VARIABLE_ATTRIBUTE _scopeAttribute;

        protected AbstractUEFIFeature(string guid, string scopeName, Kernel32.VARIABLE_ATTRIBUTE scopeAttribute)
        {
            _guid = guid;
            _scopeName = scopeName;
            _scopeAttribute = scopeAttribute;
        }

        public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

        public abstract Task<T> GetStateAsync();

        public abstract Task SetStateAsync(T state);

        protected Task<S> ReadFromUefiAsync<S>(S structure) where S : struct => Task.Run(() =>
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Reading from UEFI... [feature={GetType().Name}]");

            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<S>());

            try
            {
                if (!SetPrivilege(true))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot set UEFI privilages [feature={GetType().Name}]");

                    throw new InvalidOperationException($"Cannot set privilages UEFI");
                }

                Marshal.StructureToPtr(structure, ptr, false);
                if (Kernel32.GetFirmwareEnvironmentVariableEx(_scopeName, _guid, ptr, (uint)Marshal.SizeOf<S>(), out _) != Win32Error.ERROR_SUCCESS)
                {
                    var result = Marshal.PtrToStructure<S>(ptr);

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Read from UEFI successful [feature={GetType().Name}]");

                    return result;
                }
                else
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot read variable {_scopeName} from UEFI [feature={GetType().Name}]");

                    throw new InvalidOperationException($"Cannot read variable {_scopeName} from UEFI");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
                SetPrivilege(false);
            }
        });

        protected Task WriteToUefiAsync<S>(S structure) where S : struct => Task.Run(() =>
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<S>());

            try
            {
                if (!SetPrivilege(true))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot set UEFI privilages [feature={GetType().Name}]");

                    throw new InvalidOperationException($"Cannot set privilages UEFI");
                }

                Marshal.StructureToPtr(structure, ptr, false);
                if (!Kernel32.SetFirmwareEnvironmentVariableEx(_scopeName, _guid, ptr, (uint)Marshal.SizeOf<S>(), _scopeAttribute))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot write variable {_scopeName} to UEFI [feature={GetType().Name}]");

                    throw new InvalidOperationException($"Cannot write variable {_scopeName} to UEFI");
                }
                else
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"WriteAsync to UEFI successful [feature={GetType().Name}]");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
                SetPrivilege(false);
            }
        });

        private bool SetPrivilege(bool enable)
        {
            try
            {
                if (!AdvApi32.OpenProcessToken(Kernel32.GetCurrentProcess(), AdvApi32.TokenAccess.TOKEN_QUERY | AdvApi32.TokenAccess.TOKEN_ADJUST_PRIVILEGES, out var token))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Could not open process token [feature={GetType().Name}]");

                    return false;
                }

                if (!AdvApi32.LookupPrivilegeValue(null, "SeSystemEnvironmentPrivilege", out var luid))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Could not look up privilege value [feature={GetType().Name}]");

                    return false;
                }

                var state = new AdvApi32.TOKEN_PRIVILEGES(luid,
                    enable
                        ? AdvApi32.PrivilegeAttributes.SE_PRIVILEGE_ENABLED
                        : AdvApi32.PrivilegeAttributes.SE_PRIVILEGE_DISABLED);

                if (AdvApi32.AdjustTokenPrivileges(token, false, in state, out _) != Win32Error.ERROR_SUCCESS)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Could not adjust token privileges [feature={GetType().Name}]");

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Exception while setting privilege. [feature={GetType().Name}]", ex);

                return false;
            }
        }
    }
}
