using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Helpers.Win32
{
    internal class WindowChecks
    {
        private static bool isWindow;
        private static bool isWindowVisible;
        private static bool cloakedCheck;
        private static bool gaCheck;
        private static bool gwCheck;
        private static bool flagCheckT;
        private static bool classCheck;

        private static StringBuilder className = new StringBuilder(256);

        private static int attributeSize = Marshal.SizeOf(typeof(int));
        private static int attributeValue = 0;
        private static int hr;

        internal static bool isUserWindow(IntPtr hWnd)
        {
            // Debug code, each is declared so I can find value on breakpoint.
            /*isWindow = IsWindow(hWnd);
            isWindowVisible = IsWindowVisible(hWnd);
            cloakedCheck = !isCloaked(hWnd);
            gaCheck = GetAncestor(hWnd, (GetAncestorFlags)GA_ROOT) == hWnd;
            gwCheck = GetWindow(hWnd, (GetWindowType)GW_OWNER) == IntPtr.Zero;
            flagCheckT = flagCheck(hWnd);
            classCheck = classNameCheck(hWnd);

            if(isWindow && isWindowVisible && cloakedCheck || classCheck && gaCheck && gwCheck && flagCheckT) { return true; }
            else { return false; }*/

            if (IsWindow(hWnd) && IsWindowVisible(hWnd) && !isCloaked(hWnd) && GetAncestor(hWnd, (GetAncestorFlags)GA_ROOT) == hWnd && GetWindow(hWnd, (GetWindowType)GW_OWNER) == IntPtr.Zero && flagCheck(hWnd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool flagCheck(IntPtr hWnd)
        {
            int exStyle = (int)GetWindowLongPtr(hWnd, GWL_EXSTYLE);
            //return ((IntPtr)exStyle != IntPtr.Zero && ((exStyle & WS_EX_APPWINDOW) != 0 || ((exStyle & (WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE)) == 0)) && GetPropA(hWnd, "ITaskList_Deleted") == null);
            return (exStyle & WS_EX_APPWINDOW) == WS_EX_APPWINDOW || (exStyle & (WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE)) == 0;
        }

        private static bool isCloaked(IntPtr hWnd)
        {
            hr = DwmGetWindowAttribute(hWnd, (int)(DWMWINDOWATTRIBUTE)DWMWA_CLOAKED, out attributeValue, attributeSize);
            if (hr >= 0)
            {
                if (attributeValue == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Debug.WriteLine("WindowChecks: [-] Failed to get cloaked attribute.");
                return false;
            }
        }

        private static bool classNameCheck(IntPtr hWnd)
        {
            GetClassName(hWnd, className, className.Capacity);
            if (className.ToString() == "Windows.UI.Core.CoreWindow") { return true; }
            else { return false; }
        }
    }
}
