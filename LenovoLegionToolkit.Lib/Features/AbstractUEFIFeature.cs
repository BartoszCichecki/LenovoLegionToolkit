using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Security;

#pragma warning disable CA1416 // Validate platform compatibility

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractUEFIFeature<T> : IFeature<T> where T : struct, Enum, IComparable
    {
        private readonly string _guid;
        private readonly string _scopeName;
        private readonly uint _scopeAttribute;

        protected AbstractUEFIFeature(string guid, string scopeName, uint scopeAttribute)
        {
            _guid = guid;
            _scopeName = scopeName;
            _scopeAttribute = scopeAttribute;
        }

        public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

        public abstract Task<T> GetStateAsync();

        public abstract Task SetStateAsync(T state);

        protected unsafe Task<TS> ReadFromUefiAsync<TS>() where TS : struct => Task.Run(() =>
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Reading from UEFI... [feature={GetType().Name}]");

            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TS>());

            try
            {
                if (!SetPrivilege(true))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot set UEFI privilages [feature={GetType().Name}]");

                    throw new InvalidOperationException("Cannot set privilages UEFI");
                }

                var ptrSize = (uint)Marshal.SizeOf<TS>();
                if (PInvoke.GetFirmwareEnvironmentVariableEx(_scopeName, _guid, ptr.ToPointer(), ptrSize, null) != 0)
                {
                    var result = Marshal.PtrToStructure<TS>(ptr);

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

        protected unsafe Task WriteToUefiAsync<TS>(TS structure) where TS : struct => Task.Run(() =>
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TS>());

            try
            {
                if (!SetPrivilege(true))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot set UEFI privilages [feature={GetType().Name}]");

                    throw new InvalidOperationException("Cannot set privilages UEFI");
                }

                Marshal.StructureToPtr(structure, ptr, false);
                var ptrSize = (uint)Marshal.SizeOf<TS>();
                if (!PInvoke.SetFirmwareEnvironmentVariableEx(_scopeName, _guid, ptr.ToPointer(), ptrSize, _scopeAttribute))
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

        private unsafe bool SetPrivilege(bool enable)
        {
            try
            {
                using var handle = PInvoke.GetCurrentProcess_SafeHandle();

                if (!PInvoke.OpenProcessToken(handle, TOKEN_ACCESS_MASK.TOKEN_QUERY | TOKEN_ACCESS_MASK.TOKEN_ADJUST_PRIVILEGES, out var token))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Could not open process token [feature={GetType().Name}]");

                    return false;
                }

                if (!PInvoke.LookupPrivilegeValue(null, "SeSystemEnvironmentPrivilege", out var luid))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Could not look up privilege value [feature={GetType().Name}]");

                    return false;
                }

                var state = new TOKEN_PRIVILEGES { PrivilegeCount = 1 };
                state.Privileges._0 = new LUID_AND_ATTRIBUTES()
                {
                    Luid = luid,
                    Attributes = enable ? TOKEN_PRIVILEGES_ATTRIBUTES.SE_PRIVILEGE_ENABLED : 0
                };

                if (!PInvoke.AdjustTokenPrivileges(token, false, state, 0, null, null))
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
