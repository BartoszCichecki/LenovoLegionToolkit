using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal static class Native
    {
        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW")]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [DllImport("user32.dll")]
        public static extern bool ChangeWindowMessageFilter(uint message, uint dwFlag);
    }
}
