#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using System;
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
