using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using static GyroShell.Helpers.Win32Interop;
using System.Threading;

// Thank stackoverflow for this one

namespace GyroShell.Helpers
{
    public class TaskbarManager
    {
        public static void SetHeight(int height)
        {
            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

            NativeRect workArea = new NativeRect();

            workArea.Top = 0;
            workArea.Left = 0;
            workArea.Right = screenWidth;
            workArea.Bottom = screenHeight - height;

            //Probably will need rework when using more than 1 monitor
            SystemParametersInfoA(SPI_SETWORKAREA, 0, ref workArea, SPIF_UPDATEINIFILE);
        }

        public static void ShowTaskbar()
        {
            SetVisibility(true);
        }

        public static void HideTaskbar()
        {
            SetVisibility(false);
        }

        private static void SetVisibility(bool isVisible)
        {
            var taskbar = FindWindow("Shell_TrayWnd", null);
            var multitaskbar = FindWindow("Shell_SecondaryTrayWnd", null);
            var startmenu = FindWindowEx(taskbar, IntPtr.Zero, "Button", "Start");

            if (startmenu == IntPtr.Zero)
            {
                startmenu = FindWindow("Button", null);
            }

            int nCmd = isVisible ? SW_SHOW : SW_HIDE;

            ShowWindow(taskbar, nCmd);
            ShowWindow(startmenu, nCmd);
            ShowWindow(multitaskbar, nCmd);
        }

    }
}
