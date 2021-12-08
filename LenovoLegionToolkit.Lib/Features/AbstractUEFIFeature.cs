using System;
using System.Runtime.InteropServices;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractUEFIFeature<T> : IFeature<T>
    {
        private readonly string _guid;
        private readonly string _scopeName;
        private readonly int _scopeAttribute;

        protected AbstractUEFIFeature(string guid, string scopeName, int scopeAttribute)
        {
            _guid = guid;
            _scopeName = scopeName;
            _scopeAttribute = scopeAttribute;
        }

        public abstract T GetState();
        public abstract void SetState(T state);

        protected S ReadFromUefi<S>(S structure) where S : struct
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Reading from UEFI... [feature={GetType().Name}]");

            if (!IsUefiMode())
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"UEFI mode is not enabled. [feature={GetType().Name}]");
                
                throw new InvalidOperationException("UEFI mode is not enabled");
            }

            var hGlobal = Marshal.AllocHGlobal(Marshal.SizeOf<S>());

            try
            {
                if (!SetPrivilege(true))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot set UEFI privilages [feature={GetType().Name}]");
                    
                    throw new InvalidOperationException($"Cannot set privilages UEFI");
                }

                var ptr = hGlobal;

                Marshal.StructureToPtr(structure, ptr, false);
                if (Native.GetFirmwareEnvironmentVariableExW(_scopeName, _guid, hGlobal, Marshal.SizeOf<S>(), IntPtr.Zero) != 0)
                {
                    var result = (S)Marshal.PtrToStructure(hGlobal, typeof(S));
                    
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
                Marshal.FreeHGlobal(hGlobal);
                SetPrivilege(false);
            }
        }

        protected void WriteToUefi<S>(S structure) where S : struct
        {
            if (!IsUefiMode())
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"UEFI mode is not enabled. [feature={GetType().Name}]");
                
                throw new InvalidOperationException("UEFI mode is not enabled.");
            }

            var hGlobal = Marshal.AllocHGlobal(Marshal.SizeOf<S>());

            try
            {
                if (!SetPrivilege(true))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot set UEFI privilages [feature={GetType().Name}]");
                    
                    throw new InvalidOperationException($"Cannot set privilages UEFI");
                }

                var ptr = hGlobal;
                Marshal.StructureToPtr(structure, ptr, false);
                if (Native.SetFirmwareEnvironmentVariableExW(_scopeName, _guid, hGlobal, Marshal.SizeOf<S>(), _scopeAttribute) != 1)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Cannot write variable {_scopeName} to UEFI [feature={GetType().Name}]");
                    
                    throw new InvalidOperationException($"Cannot write variable {_scopeName} to UEFI");
                }
                else
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Write to UEFI successful [feature={GetType().Name}]");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(hGlobal);
                SetPrivilege(false);
            }
        }

        private bool IsUefiMode()
        {
            var firmwareType = FirmwareType.Unknown;
            if (Native.GetFirmwareType(ref firmwareType))
            {
                var result = firmwareType == FirmwareType.Uefi;
                
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Firmware type is {firmwareType} [feature={GetType().Name}]");
                
                return result;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Could not get firmware type [feature={GetType().Name}]");
            
            return false;
        }

        private bool SetPrivilege(bool enable)
        {
            try
            {
                var zero = IntPtr.Zero;
                if (!Native.OpenProcessToken(Native.GetCurrentProcess(), 40U, ref zero))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Could not open process token [feature={GetType().Name}]");
                    
                    return false;
                }

                TokenPrivelege newState;
                newState.Count = 1;
                newState.Luid = 0L;
                newState.Attr = enable ? 2 : 0;
                if (!Native.LookupPrivilegeValue(null, "SeSystemEnvironmentPrivilege", ref newState.Luid))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Could not look up privilege value [feature={GetType().Name}]");
                    
                    return false;
                }

                if (!Native.AdjustTokenPrivileges(zero, false, ref newState, 0, IntPtr.Zero, IntPtr.Zero))
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
                    Log.Instance.Trace($"Exception: {ex} [feature={GetType().Name}]");
                
                return false;
            }
        }
    }
}
