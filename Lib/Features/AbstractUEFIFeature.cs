using LenovoLegionToolkit.Lib.Utils;
using System;
using System.Runtime.InteropServices;

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
            if (!IsUefiMode())
                throw new InvalidOperationException("UEFI mode is not enabled.");

            var hGlobal = Marshal.AllocHGlobal(Marshal.SizeOf<S>());

            try
            {
                if (!SetPrivilege(true))
                    throw new InvalidOperationException($"Cannot set privilages UEFI.");

                var ptr = hGlobal;

                Marshal.StructureToPtr(structure, ptr, false);
                if (Native.GetFirmwareEnvironmentVariableExW(_scopeName, _guid, hGlobal, Marshal.SizeOf<S>(), IntPtr.Zero) != 0)
                    return (S)Marshal.PtrToStructure(hGlobal, typeof(S));
                else
                    throw new InvalidOperationException($"Cannot read variable {_scopeName} from UEFI.");
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
                throw new InvalidOperationException("UEFI mode is not enabled.");

            var hGlobal = Marshal.AllocHGlobal(Marshal.SizeOf<S>());

            try
            {
                if (!SetPrivilege(true))
                    throw new InvalidOperationException($"Cannot set privilages UEFI.");

                var ptr = hGlobal;
                Marshal.StructureToPtr(structure, ptr, false);
                if (Native.SetFirmwareEnvironmentVariableExW(_scopeName, _guid, hGlobal, Marshal.SizeOf<S>(), _scopeAttribute) != 1)
                    throw new InvalidOperationException($"Cannot write variable {_scopeName} to UEFI.");
            }
            finally
            {
                Marshal.FreeHGlobal(hGlobal);
                SetPrivilege(false);
            }
        }

        private static bool IsUefiMode()
        {
            var firmwareType = FirmwareType.FirmwareTypeUnknown;
            if (Native.GetFirmwareType(ref firmwareType))
                return firmwareType == FirmwareType.FirmwareTypeUefi;

            return false;
        }

        private static bool SetPrivilege(bool enable)
        {
            try
            {
                var zero = IntPtr.Zero;
                if (!Native.OpenProcessToken(Native.GetCurrentProcess(), 40U, ref zero))
                {
                    return false;
                }
                TokenPrivelege newState;
                newState.Count = 1;
                newState.Luid = 0L;
                newState.Attr = enable ? 2 : 0;
                if (!Native.LookupPrivilegeValue(null, "SeSystemEnvironmentPrivilege", ref newState.Luid))
                    return  false;
                if (!Native.AdjustTokenPrivileges(zero, false, ref newState, 0, IntPtr.Zero, IntPtr.Zero))
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
