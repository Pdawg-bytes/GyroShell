#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
                GetWindow(hWnd, (GetWindowType)GW_OWNER) == IntPtr.Zero && FlagCheck(hWnd)
                && !ClassNameCheck(hWnd))
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

            if (className.ToString() == "ApplicationFrameWindow" || className.ToString() == "Windows.UI.Core.CoreWindow")
            {
                if (((int)GetWindowLongPtr(hWnd, GWL_EXSTYLE) & WS_EX_WINDOWEDGE) == 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }
}
