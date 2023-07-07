using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Helpers
{
    public class TaskbarManager
    {
        private const int ID_TRAY_SHOW_OVERFLOW = 0x028a;
        private const int ID_TRAY_HIDE_OVERFLOW = 0x028b;
        private const uint EVENT_MODIFY_STATE = 0x0002;

        private static IntPtr m_hTaskBar;
        private static IntPtr m_hMultiTaskBar;
        private static IntPtr m_hStartMenu;

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

        /*public static void SetHeight(int left, int right, int top, int bottom)
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
        }*/

        public static void ShowTaskbar()
        {
            SetVisibility(true);
        }

        public static void HideTaskbar()
        {
            SetVisibility(false);
        }

        /// <summary>
        /// Sets visibility of taskbar
        /// </summary>
        private static void SetVisibility(bool isVisible)
        {
            int nCmd = isVisible ? SW_SHOW : SW_HIDE;

            if (!isVisible)
            {
                SetWindowPos(m_hTaskBar, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, (int)SWPFlags.SWP_HIDEWINDOW | (int)SWPFlags.SWP_NOMOVE | (int)SWPFlags.SWP_NOSIZE | (int)SWPFlags.SWP_NOACTIVATE);
                SetWindowPos(m_hMultiTaskBar, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, (int)SWPFlags.SWP_HIDEWINDOW | (int)SWPFlags.SWP_NOMOVE | (int)SWPFlags.SWP_NOSIZE | (int)SWPFlags.SWP_NOACTIVATE);
            }
            else
            {
                SetWindowPos(m_hTaskBar, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 48, 0, 0, (int)SWPFlags.SWP_SHOWWINDOW);
                SetWindowPos(m_hMultiTaskBar, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 48, 0, 0, (int)SWPFlags.SWP_SHOWWINDOW);
            }

            //ShowWindow(m_hTaskBar, nCmd);
            //ShowWindow(m_hStartMenu, nCmd);
            //ShowWindow(m_hMultiTaskBar, nCmd);
        }

        /// <summary>
        /// Opens start menu
        /// </summary>
        public static async Task ToggleStart()
        {
            SendMessage(m_hTaskBar, /*WM_SYSCOMMAND*/ 0x0112, (IntPtr) /*SC_TASKLIST*/ 0xF130, (IntPtr)0);
        }

        public static async Task ShowSysTray()
        {
            SendMessage(m_hTaskBar, /*WM_COMMAND*/ 0x0111, (IntPtr)ID_TRAY_SHOW_OVERFLOW, IntPtr.Zero);
        }

        /// <summary>
        /// Opens control center
        /// </summary>
        public static async Task ToggleSysControl()
        {
            ShellExecute(IntPtr.Zero, "open", "ms-actioncenter:controlcenter/&suppressAnimations=false&showFooter=true&allowPageNavigation=true" /* CNTRLCTR, bool, bool, bool */, null, null, 1);
        }

        /// <summary>
        /// Opens action center
        /// </summary>
        public static async Task ToggleActionCenter()
        {
            ShellExecute(IntPtr.Zero, "open", "ms-actioncenter:" /* ACTION CENTER */, null, null, 1);
        }

        /// <summary>
        /// Signal to Winlogon that the shell has started and the login screen can be dismissed
        /// </summary>
        public static void SendWinlogonShowShell()
        {
            IntPtr handle = OpenEvent(EVENT_MODIFY_STATE, false, "msgina: ShellReadyEvent");
            if (handle != IntPtr.Zero)
            {
                SetEvent(handle);
                CloseHandle(handle);
            }
        }

        internal static void AutoHideExplorer(bool doHide)
        {
            if (doHide)
            {
                // MainBar
                APPBARDATA abd = new APPBARDATA();
                abd.cbSize = Marshal.SizeOf(abd);
                abd.hWnd = m_hTaskBar;
                abd.lParam = (IntPtr)ABState.ABS_AUTOHIDE;

                SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abd);

                // MultiBar
                /*if(m_hTaskBar != IntPtr.Zero)
                {
                    APPBARDATA abdM = new APPBARDATA();
                    abd.cbSize = Marshal.SizeOf(abdM);
                    abd.hWnd = m_hMultiTaskBar;
                    abd.lParam = (IntPtr)ABState.ABS_AUTOHIDE;

                    SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abdM);
                }*/
            }
            else
            {
                // MainBar
                APPBARDATA abd = new APPBARDATA();
                abd.cbSize = Marshal.SizeOf(abd);
                abd.hWnd = m_hTaskBar;
                abd.lParam = (IntPtr)ABState.ABS_TOP;

                SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abd);

                // MultiBar
                if (m_hTaskBar != IntPtr.Zero)
                {
                    APPBARDATA abdM = new APPBARDATA();
                    abd.cbSize = Marshal.SizeOf(abdM);
                    abd.hWnd = m_hMultiTaskBar;
                    abd.lParam = (IntPtr)ABState.ABS_TOP;

                    SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abdM);
                }
            }
        }
    }
}
