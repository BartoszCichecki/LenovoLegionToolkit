using System;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractUEFIFeature<T> : IFeature<T>
    {
        protected abstract string Guid { get; }
        protected abstract string ScopeName { get; }
        protected abstract int ScopeAttribute { get; }

        public abstract T GetState();
        public abstract void SetState(T state);

        protected S ReadFromUefi<S>(S structure) where S : struct
        {
            if (!IsUefiMode())
                return default;

            var num2 = Marshal.AllocHGlobal(Marshal.SizeOf<S>());

            try
            {
                if (!SetPrivilege(true))
                    return default;

                S result;
                var ptr = num2;

                Marshal.StructureToPtr(structure, ptr, false);
                if (Native.GetFirmwareEnvironmentVariableExW(ScopeName, Guid, num2, Marshal.SizeOf<S>(), IntPtr.Zero) != 0)
                    result = ((S)Marshal.PtrToStructure(num2, typeof(S)));
                else
                    result = default;

                return result;
            }
            finally
            {
                Marshal.FreeHGlobal(num2);
                SetPrivilege(false);
            }
        }

        protected void WriteToUefi<S>(S structure) where S : struct
        {
            if (!IsUefiMode())
                return;

            var num2 = Marshal.AllocHGlobal(Marshal.SizeOf<S>());

            try
            {
                if (!SetPrivilege(true))
                    return;

                IntPtr ptr = num2;
                Marshal.StructureToPtr(structure, ptr, false);
                Native.SetFirmwareEnvironmentVariableExW(ScopeName, Guid, num2, Marshal.SizeOf<S>(), ScopeAttribute);
            }
            finally
            {
                Marshal.FreeHGlobal(num2);
                SetPrivilege(false);
            }
        }

        private bool IsUefiMode()
        {
            var firmwareType = FirmwareType.FirmwareTypeUnknown;
            if (Native.GetFirmwareType(ref firmwareType))
                return firmwareType == FirmwareType.FirmwareTypeUefi;

            return false;
        }

        private bool SetPrivilege(bool enable)
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
                    return false;
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
