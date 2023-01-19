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
using System.Reflection.Metadata;
using WindowsUdk.UI.Shell;
using Windows.UI.Core;
using Windows.Foundation;
using System.Threading.Tasks;
// Thank stackoverflow for this one

namespace GyroShell.Helpers
{
    public class TaskbarManager
    {
        public static void Init()
        {
            m_hTaskBar = FindWindow("Shell_TrayWnd", null);
            m_hMultiTaskBar = FindWindow("Shell_SecondaryTrayWnd", null);
            m_hStartMenu = FindWindowEx(m_hStartMenu, IntPtr.Zero, "Button", "Start");

            if (m_hStartMenu == IntPtr.Zero)
            {
                m_hStartMenu = FindWindow("Button", null);
            }
        }

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
            int nCmd = isVisible ? SW_SHOW : SW_HIDE;

            ShowWindow(m_hTaskBar, nCmd);
            ShowWindow(m_hStartMenu, nCmd);
            ShowWindow(m_hMultiTaskBar, nCmd);
        }

        public static async Task ToggleStart()
        {
            if (OSVersion.IsWin11())
            {
                ShellViewCoordinator startC = new ShellViewCoordinator(ShellView.Start);
                await startC.TryShowAsync(new ShowShellViewOptions());
            }
            else
            {
                SendMessage(m_hTaskBar, /*WM_SYSCOMMAND*/ 0x0112, (IntPtr) /*SC_TASKLIST*/ 0xF130, (IntPtr)0);
            }
        }

        private static IntPtr m_hTaskBar;
        private static IntPtr m_hMultiTaskBar;
        private static IntPtr m_hStartMenu;

    }
}
