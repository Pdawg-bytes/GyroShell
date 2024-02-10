using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Library.Helpers.Win32
{
    public class WindowChecks
    {
        private static StringBuilder className = new StringBuilder(256);

        private static int attributeSize = Marshal.SizeOf(typeof(int));
        private static int attributeValue = 0;
        private static int hr;

        public static bool IsUserWindow(IntPtr hWnd)
        {
            if (IsWindow(hWnd) && IsWindowVisible(hWnd) && 
                !IsCloaked(hWnd) && GetAncestor(hWnd, (GetAncestorFlags)GA_ROOT) == hWnd && 
                GetWindow(hWnd, (GetWindowType)GW_OWNER) == IntPtr.Zero && FlagCheck(hWnd))
            {
                return true;
            }
            else
            {
                return false;
            }

            /*int extendedWindowStyles = (int)GetWindowLongPtr(hWnd, GWL_EXSTYLE);
            bool isWindow = IsWindow(hWnd);
            bool isVisible = IsWindowVisible(hWnd);
            bool isToolWindow = (extendedWindowStyles & WS_EX_TOOLWINDOW) != 0;
            bool isAppWindow = (extendedWindowStyles & WS_EX_APPWINDOW) != 0;
            bool isNoActivate = (extendedWindowStyles & WS_EX_NOACTIVATE) != 0;
            IntPtr ownerWin = GetWindow(hWnd, (GetWindowType)GW_OWNER);

            return isWindow && isVisible && (ownerWin == IntPtr.Zero || isAppWindow) && (!isNoActivate || isAppWindow) && !isToolWindow && !IsCloaked(hWnd);*/
        }

        private static bool FlagCheck(IntPtr hWnd)
        {
            int exStyle = (int)GetWindowLongPtr(hWnd, GWL_EXSTYLE);

            //return ((IntPtr)exStyle != IntPtr.Zero && ((exStyle & WS_EX_APPWINDOW) != 0 || ((exStyle & (WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE)) == 0)) && GetPropA(hWnd, "ITaskList_Deleted") == null);
            return (exStyle & WS_EX_APPWINDOW) == WS_EX_APPWINDOW || (exStyle & (WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE)) == 0;
        }

        private static bool IsCloaked(IntPtr hWnd)
        {
            hr = DwmGetWindowAttribute(hWnd, (int)(DWMWINDOWATTRIBUTE)DWMWA_CLOAKED, out attributeValue, attributeSize);

            if (attributeValue == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool ClassNameCheck(IntPtr hWnd)
        {
            GetClassName(hWnd, className, className.Capacity);

            if (className.ToString() == "Windows.UI.Core.CoreWindow") return true;
            else return false; 
        }
    }
}
