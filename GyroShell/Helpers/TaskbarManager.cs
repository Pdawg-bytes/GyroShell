using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

// Thanks stackoverflow for this one

namespace GyroShell.Helpers
{
    public class TaskbarManager
    {
        #region -- WIN32 Shizzle

        private const int CONST_SHOW = 5;
        private const int CONST_HIDE = 0;

        private delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumThreadWindows(int threadId, EnumThreadProc pfnEnum, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        #endregion

        public static void ShowTaskbar()
        {
            ToggleVisibility(true);
        }

        public static void HideTaskbar()
        {
            ToggleVisibility(false);
        }

        private static void ToggleVisibility(bool visible)
        {
            var taskbar = FindWindow("Shell_TrayWnd", null);

            var startmenu = FindWindowEx(taskbar, IntPtr.Zero, "Button", "Start");

            if (startmenu == IntPtr.Zero)
            {
                startmenu = FindWindow("Button", null);
            }

            ShowWindow(taskbar, visible ? CONST_SHOW : CONST_HIDE);
            ShowWindow(startmenu, visible ? CONST_SHOW : CONST_HIDE);
        }

    }
}
