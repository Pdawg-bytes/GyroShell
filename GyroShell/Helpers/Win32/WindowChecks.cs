using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Chat;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Helpers.Win32
{
    internal class WindowChecks
    {
        private static int attributeValue;
        private static int attributeSize = Marshal.SizeOf(typeof(int));
        internal static bool isUserWindow(IntPtr hWnd)
        {
            if (IsWindow(hWnd) && IsWindowVisible(hWnd) && !isCloaked(hWnd) && GetAncestor(hWnd, (GetAncestorFlags)GA_ROOT) == hWnd && GetWindow(hWnd, (GetWindowType)GW_OWNER) == IntPtr.Zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool isCloaked(IntPtr hWnd)
        {
            DwmGetWindowAttribute(hWnd, (int)(DWMWINDOWATTRIBUTE)DWMWA_CLOAKED, out attributeValue, attributeSize);
            if (attributeValue == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
