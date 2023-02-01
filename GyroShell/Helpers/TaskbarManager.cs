using System;
using static GyroShell.Helpers.Win32Interop;
using System.Threading.Tasks;

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

        public static void SetHeight(int left, int right, int top, int bottom)
        {
            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

            NativeRect workArea = new NativeRect();

            workArea.Top = top;
            workArea.Left = left;
            workArea.Right = screenWidth - right;
            workArea.Bottom = screenHeight - bottom;

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
            SendMessage(m_hTaskBar, /*WM_SYSCOMMAND*/ 0x0112, (IntPtr) /*SC_TASKLIST*/ 0xF130, (IntPtr)0);
        }

        public static async Task ToggleSysControl()
        {
            ShellExecute(IntPtr.Zero, "open", "ms-actioncenter:controlcenter/&suppressAnimations=false&showFooter=true&allowPageNavigation=true", null, null, 1);
        }

        public static async Task ToggleActionCenter()
        {
            ShellExecute(IntPtr.Zero, "open", "ms-actioncenter:", null, null, 1);
        }

        private static IntPtr m_hTaskBar;
        private static IntPtr m_hMultiTaskBar;
        private static IntPtr m_hStartMenu;

    }
}
