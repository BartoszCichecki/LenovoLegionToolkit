using System.Linq;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Security;

namespace LenovoLegionToolkit.Lib.Utils;

public static class TokenManipulator
{
    public const string SE_BACKUP_PRIVILEGE = "SeBackupPrivilege";
    public const string SE_RESTORE_PRIVILEGE = "SeRestorePrivilege";
    public const string SE_TAKE_OWNERSHIP_PRIVILEGE = "SeTakeOwnershipPrivilege";
    public const string SE_SYSTEM_ENVIRONMENT_PRIVILEGE = "SeSystemEnvironmentPrivilege";

    public static bool AddPrivileges(params string[] privileges) => AdjustPrivileges(privileges, true);

    public static bool RemovePrivileges(params string[] privileges) => AdjustPrivileges(privileges, true);

    private static bool AdjustPrivileges(string[] privileges, bool enable)
    {
        SafeProcessHandle? safeHandle = null;
        SafeFileHandle? safeTokenHandle = null;

        try
        {
            safeHandle = new SafeProcessHandle(PInvoke.GetCurrentProcess(), true);
            if (!PInvoke.OpenProcessToken(safeHandle, TOKEN_ACCESS_MASK.TOKEN_ADJUST_PRIVILEGES | TOKEN_ACCESS_MASK.TOKEN_QUERY, out safeTokenHandle) && safeTokenHandle is null)
                return false;

            return privileges.All(p => AdjustPrivilege(safeTokenHandle, p, enable));
        }
        finally
        {
            safeTokenHandle?.Dispose();
            safeHandle?.Dispose();
        }
    }

    private static unsafe bool AdjustPrivilege(SafeFileHandle safeTokenHandle, string privilegeName, bool enable)
    {
        PInvoke.LookupPrivilegeValue(null, privilegeName, out var luid);

        var state = new TOKEN_PRIVILEGES { PrivilegeCount = 1 };
        state.Privileges[0] = new LUID_AND_ATTRIBUTES
        {
            Luid = luid,
            Attributes = enable ? TOKEN_PRIVILEGES_ATTRIBUTES.SE_PRIVILEGE_ENABLED : 0
        };

        return PInvoke.AdjustTokenPrivileges(safeTokenHandle, false, &state, 0, null, null);
    }
}
