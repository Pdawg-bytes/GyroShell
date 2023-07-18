using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Helpers.Win32
{
    internal class ScreenValues
    {
        internal static int GetScreenHeight()
        {
            return GetSystemMetrics(SM_CYSCREEN);
        }

        internal static int GetScreenWidth()
        {
            return GetSystemMetrics(SM_CXSCREEN);
        }
    }
}
