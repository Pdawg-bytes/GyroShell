using System;
using static GyroShell.Helpers.Win32.Win32Interop;
using System.Text;

namespace GyroShell.Helpers.Win32
{
    internal class GetWindowName
    {
        internal static string GetWindowTitle(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
