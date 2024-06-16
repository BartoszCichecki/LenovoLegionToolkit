using System;
using System.Linq;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace LenovoLegionToolkit.Lib.System;

public static class SystemPath
{
    private static string CLIPath => Folders.Program;

    public static bool HasCLI()
    {
        return Registry.GetValue("HKEY_CURRENT_USER", "Environment", "PATH", string.Empty, true)
            .Split(';')
            .Contains(CLIPath);
    }

    public static void SetCLI(bool enabled)
    {
        var value = Registry.GetValue("HKEY_CURRENT_USER", "Environment", "PATH", string.Empty, true)
            .Split(';')
            .ToList();

        if (enabled)
        {
            if (value.Contains(CLIPath))
                return;

            value.Add(CLIPath);
        }
        else
        {
            value.Remove(CLIPath);
        }

        Registry.SetValue("HKEY_CURRENT_USER",
            "Environment",
            "PATH",
            string.Join(';', value),
            valueKind: RegistryValueKind.ExpandString);

        Notify();
    }

    private static unsafe void Notify()
    {
        const string environment = "Environment";
        fixed (void* ptr = environment)
        {
            PInvoke.SendNotifyMessage(HWND.HWND_BROADCAST, PInvoke.WM_SETTINGCHANGE, 0, new IntPtr(ptr));
        }
    }
}
