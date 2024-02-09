using System;
using System.Diagnostics;
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
            // Debug code, each is declared so I can find value on breakpoint.
            /*
            bool isWindow = IsWindow(hWnd);
            bool isWindowVisible = IsWindowVisible(hWnd);
            bool cloakedCheck = !isCloaked(hWnd);
            bool gaCheck = GetAncestor(hWnd, (GetAncestorFlags)GA_ROOT) == hWnd;
            bool gwCheck = GetWindow(hWnd, (GetWindowType)GW_OWNER) == IntPtr.Zero;
            bool flagCheckT = flagCheck(hWnd);
            bool classCheck = classNameCheck(hWnd);

            if(isWindow && isWindowVisible && cloakedCheck || classCheck && gaCheck && gwCheck && flagCheckT) { return true; }
            else { return false; }*/

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

        private static bool ClassNameCheck(IntPtr hWnd)
        {
            GetClassName(hWnd, className, className.Capacity);

            if (className.ToString() == "Windows.UI.Core.CoreWindow") return true;
            else return false; 
        }
    }
}
