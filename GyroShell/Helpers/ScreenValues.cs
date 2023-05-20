using static GyroShell.Helpers.Win32Interop;

namespace GyroShell.Helpers
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
