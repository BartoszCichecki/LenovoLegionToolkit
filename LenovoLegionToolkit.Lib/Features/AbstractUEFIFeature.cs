using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractUEFIFeature<T>(string guid, string scopeName, uint scopeAttribute)
    : IFeature<T> where T : struct, Enum, IComparable
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            _ = await GetStateAsync().ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
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
            if (!TokenManipulator.AddPrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges [feature={GetType().Name}]");

                throw new InvalidOperationException("Cannot set privileges UEFI");
            }

            var ptrSize = (uint)Marshal.SizeOf<TS>();
            if (PInvoke.GetFirmwareEnvironmentVariableEx(scopeName, guid, ptr.ToPointer(), ptrSize, null) != 0)
            {
                var result = Marshal.PtrToStructure<TS>(ptr);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Read from UEFI successful [feature={GetType().Name}]");

                return result;
            }
            else
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot read variable {scopeName} from UEFI [feature={GetType().Name}]");

                throw new InvalidOperationException($"Cannot read variable {scopeName} from UEFI");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
            TokenManipulator.RemovePrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE);
        }
    });

    protected unsafe Task WriteToUefiAsync<TS>(TS structure) where TS : struct => Task.Run(() =>
    {
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TS>());

        try
        {
            if (!TokenManipulator.AddPrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges [feature={GetType().Name}]");

                throw new InvalidOperationException("Cannot set UEFI privileges");
            }

            Marshal.StructureToPtr(structure, ptr, false);
            var ptrSize = (uint)Marshal.SizeOf<TS>();
            if (!PInvoke.SetFirmwareEnvironmentVariableEx(scopeName, guid, ptr.ToPointer(), ptrSize, scopeAttribute))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot write variable {scopeName} to UEFI [feature={GetType().Name}]");

                throw new InvalidOperationException($"Cannot write variable {scopeName} to UEFI");
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
            TokenManipulator.RemovePrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE);
        }
    });
}
