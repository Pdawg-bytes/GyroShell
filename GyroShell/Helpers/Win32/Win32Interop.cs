using ABI.Windows.Foundation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace GyroShell.Helpers.Win32
{
    internal static class Win32Interop
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeRect
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }

        internal const int SW_RESTORE = 9;
        internal const int SW_SHOW = 5;
        internal const int SW_HIDE = 0;

        internal const int SPI_SETWORKAREA = 0x002F;
        internal const int SPI_GETWORKAREA = 0x0030;

        internal const int SPIF_UPDATEINIFILE = 1;

        internal const int GWL_EXSTYLE = -20;
        internal const int GWL_STYLE = -16;

        internal const int DWMWA_CLOAKED = 14;

        internal const int WS_CHILD = 0x40000000;
        internal const int GCL_HICONSM = -34;
        internal const int GCL_HICON = -14;

        internal const int ICON_SMALL = 0;
        internal const int ICON_BIG = 1;
        internal const int ICON_SMALL2 = 2;
        internal const int WM_GETICON = 0x7F;

        internal const uint GA_PARENT = 1;
        internal const uint GA_ROOT = 2;
        internal const uint GA_ROOTOWNER = 3;

        internal const int WS_EX_APPWINDOW = 0x00040000;
        internal const int WS_EX_TOOLWINDOW = 0x00000080;
        internal const int WS_EX_NOACTIVATE = 0x08000000;

        internal const int HSHELL_WINDOWCREATED = 1;
        internal const int HSHELL_WINDOWDESTROYED = 2;
        internal const int HSHELL_ACTIVATESHELLWINDOW = 3;
        internal const int HSHELL_WINDOWACTIVATED = 4;
        internal const int HSHELL_GETMINRECT = 5;
        internal const int HSHELL_REDRAW = 6;
        internal const int HSHELL_TASKMAN = 7;
        internal const int HSHELL_LANGUAGE = 8;
        internal const int HSHELL_SYSMENU = 9;
        internal const int HSHELL_ENDTASK = 10;
        internal const int HSHELL_ACCESSIBILITYSTATE = 11;
        internal const int HSHELL_APPCOMMAND = 12;
        internal const int FAPPCOMMAND_MASK = 0xF000;
        internal const int HSHELL_WINDOWREPLACED = 13;
        internal const int HSHELL_WINDOWREPLACING = 14;
        internal const int HSHELL_MONITORCHANGED = 16; //A window is moved to a different monitor.

        // Unofficial HSHELL codes
        internal const int HSHELL_FULLSCREEN_ENABLED = 53;
        internal const int HSHELL_FULLSCREEN_DISABLED = 54;

        internal const int GWLP_WNDPROC = -4;
        internal const int SM_CXSCREEN = 0;
        internal const int SM_CYSCREEN = 1;

        internal const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        internal const uint WINEVENT_SKIPOWNTHREAD = 0x0001;
        internal const uint EVENT_SYSTEM_DESKTOPSWITCH = 0x0020;
        internal const uint EVENT_OBJECT_CLOAKED = 0x8017;
        internal const uint EVENT_OBJECT_UNCLOAKED = 0x8018;
        internal const int EVENT_OBJECT_NAMECHANGED = 0x800C;
        internal const int EVENT_OBJECT_DESTROY = 0x8001;
        internal const int EVENT_OBJECT_CREATE = 0x8000;
        internal const int WINEVENT_INCONTEXT = 4;
        internal const int WINEVENT_SKIPOWNPROCESS = 2;
        internal const int EVENT_SYSTEM_FOREGROUND = 3;
        internal const int WH_SHELL = 10;
        internal const int GW_OWNER = 4;
        internal const long OBJID_WINDOW = 0x00000000L;


        internal delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);
        internal enum DwmWindowAttribute : int
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation,
            PassiveUpdateMode,
            UseHostBackdropBrush,
            UseImmersiveDarkMode = 20,
            WindowCornerPreference = 33,
            BorderColor,
            CaptionColor,
            TextColor,
            VisibleFrameBorderThickness,
            SystemBackdropType,
            Last
        }

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags);
        internal enum GetAncestorFlags
        {
            /// <summary>
            /// Retrieves the parent window. This does not include the owner, as it does with the GetParent function.
            /// </summary>
            GetParent = 1,
            /// <summary>
            /// Retrieves the root window by walking the chain of parent windows.
            /// </summary>
            GetRoot = 2,
            /// <summary>
            /// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.
            /// </summary>
            GetRootOwner = 3
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);
        internal enum GetWindowType : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);


        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindowIcon(IntPtr hWnd, int flags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DestroyIcon(IntPtr hIcon);

        internal enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }


        [DllImport("user32.dll")]
        internal static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetPropA(IntPtr hwnd, string name);

        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(EnumWindowsCallback lpEnumFunc, IntPtr lParam);
        internal delegate bool EnumWindowsCallback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        internal delegate IntPtr ShellProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowsHookEx(int idHook, ShellProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsTopLevelWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        internal static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        internal delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        internal static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("kernel32.dll")]
        internal static extern bool SetEvent(IntPtr hEvent);
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SystemParametersInfoA(uint uiAction, uint uiParam, ref NativeRect pvParam, uint fWinIni);

        internal delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData);

        [DllImport("User32.dll")]
        internal static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        [DllImport("User32.dll")]
        internal static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")] // 32-bit Eq. of SetWindowLongPtr
        internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool RegisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool DeregisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

        // DWM API attrib
        internal enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }
        // Copied from dwmapi.h
        internal enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }
        // DwmSetWindowAttribute
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

        // Proc info API for unpackaged
        [DllImport("kernel32.dll")]
        internal static extern void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("shell32.dll", SetLastError = true, EntryPoint ="#188")]
        internal static extern bool ShellDDEInit(bool init);
        [DllImport("shell32.dll", SetLastError = true, EntryPoint ="#181")]
        internal static extern bool RegisterShellHook(IntPtr hwnd, int fInstall);
        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            internal ushort wProcessorArchitecture;
            internal ushort wReserved;
            internal uint dwPageSize;
            internal IntPtr lpMinimumApplicationAddress;
            internal IntPtr lpMaximumApplicationAddress;
            internal IntPtr dwActiveProcessorMask;
            internal uint dwNumberOfProcessors;
            internal uint dwProcessorType;
            internal uint dwAllocationGranularity;
            internal ushort wProcessorLevel;
            internal ushort wProcessorRevision;
        }

        // APPBAR code
        [StructLayout(LayoutKind.Sequential)]
        internal struct APPBARDATA
        {
            internal int cbSize;
            internal IntPtr hWnd;
            internal int uCallbackMessage;
            internal int uEdge;
            internal RECT rc;
            internal IntPtr lParam;
        }
        internal enum ABMsg : int
        {
            ABM_NEW = 0,
            ABM_REMOVE,
            ABM_QUERYPOS,
            ABM_SETPOS,
            ABM_GETSTATE,
            ABM_GETTASKBARPOS,
            ABM_ACTIVATE,
            ABM_GETAUTOHIDEBAR,
            ABM_SETAUTOHIDEBAR,
            ABM_WINDOWPOSCHANGED,
            ABM_SETSTATE
        }
        internal enum ABNotify : int
        {
            ABN_STATECHANGE = 0,
            ABN_POSCHANGED,
            ABN_FULLSCREENAPP,
            ABN_WINDOWARRANGE
        }
        internal enum ABEdge : int
        {
            ABE_LEFT = 0,
            ABE_TOP,
            ABE_RIGHT,
            ABE_BOTTOM
        }

        internal enum ABState
        {
            ABS_TOP = 0x00,
            ABS_AUTOHIDE = 0x01
        }

        internal delegate IntPtr WndProcDelegate(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        internal static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int RegisterWindowMessage(string msg);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        internal enum SWPFlags
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_DEFERERASE = 0x2000,
            SWP_ASYNCWINDOWPOS = 0x4000
        }
        internal enum WindowZOrder
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2,
        }
    }
}