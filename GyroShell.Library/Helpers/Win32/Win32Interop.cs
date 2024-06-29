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
using Windows.ApplicationModel;
using static GyroShell.Library.Interfaces.IPropertyStoreAUMID;

namespace GyroShell.Library.Helpers.Win32
{
    public static class Win32Interop
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public const int WM_COMMAND = 0x0111;
        public const int WM_SYSCOMMAND = 0x0112;

        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MOVE = 0xF010;
        public const int SC_RESTORE = 0xF120;
        public const int SC_SIZE = 0xF000;
        public const int SC_CLOSE = 0xF060;

        public const int SW_RESTORE = 9;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOW = 5;
        public const int SW_HIDE = 0;

        public const int SPI_SETWORKAREA = 0x002F;
        public const int SPI_GETWORKAREA = 0x0030;

        public const int SPIF_UPDATEINIFILE = 1;

        public const int GWL_EXSTYLE = -20;
        public const int GWL_STYLE = -16;

        public const int DWMWA_CLOAKED = 14;

        public const int GCL_HICONSM = -34;
        public const int GCL_HICON = -14;

        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL2 = 2;
        public const int WM_GETICON = 0x7F;

        public const uint GA_PARENT = 1;
        public const uint GA_ROOT = 2;
        public const uint GA_ROOTOWNER = 3;

        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int WS_EX_WINDOWEDGE = 0x100;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_CHILD = 0x40000000;

        public const int HSHELL_WINDOWCREATED = 1;
        public const int HSHELL_WINDOWDESTROYED = 2;
        public const int HSHELL_ACTIVATESHELLWINDOW = 3;
        public const int HSHELL_WINDOWACTIVATED = 4;
        public const int HSHELL_RUDEAPPACTIVATED = 32772;
        public const int HSHELL_GETMINRECT = 5;
        public const int HSHELL_REDRAW = 6;
        public const int HSHELL_TASKMAN = 7;
        public const int HSHELL_LANGUAGE = 8;
        public const int HSHELL_SYSMENU = 9;
        public const int HSHELL_ENDTASK = 10;
        public const int HSHELL_ACCESSIBILITYSTATE = 11;
        public const int HSHELL_APPCOMMAND = 12;
        public const int HSHELL_WINDOWREPLACED = 13;
        public const int HSHELL_WINDOWREPLACING = 14;
        public const int HSHELL_MONITORCHANGED = 16; //A window is moved to a different monitor.
        public const int HSHELL_HIGHBIT = 0x8000;
        public const int HSHELL_FLASH = 0x8006;

        public const int FAPPCOMMAND_MASK = 0xF000;

        public const int GWLP_WNDPROC = -4;
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        public const int WINEVENT_OUTOFCONTEXT = 0x0000;
        public const int WINEVENT_SKIPOWNTHREAD = 0x0001;
        public const int EVENT_SYSTEM_DESKTOPSWITCH = 0x0020;
        public const int EVENT_OBJECT_CLOAKED = 0x8017;
        public const int EVENT_OBJECT_UNCLOAKED = 0x8018;
        public const int EVENT_OBJECT_NAMECHANGED = 0x800C;
        public const int EVENT_OBJECT_DESTROY = 0x8001;
        public const int EVENT_OBJECT_CREATE = 0x8000;
        public const int WINEVENT_INCONTEXT = 4;
        public const int WINEVENT_SKIPOWNPROCESS = 2;
        public const int EVENT_SYSTEM_FOREGROUND = 3;
        public const int WH_SHELL = 10;
        public const int GW_OWNER = 4;
        public const long OBJID_WINDOW = 0x00000000L;

        public const uint EWX_LOGOFF = 0x00000000;

        public delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);
        public enum DwmWindowAttribute : int
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
        public static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags);
        public enum GetAncestorFlags
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
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);
        public enum GetWindowType : uint
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
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);
        public enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        [DllImport("PowrProf.dll")]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32.dll")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("ole32.dll")]
        public static extern int PropVariantClear(ref Interfaces.IPropertyStoreAUMID.PropVariant pvar);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetPackageFamilyName(IntPtr hProcess, ref uint packageFamilyNameLength, [Out] char[] packageFamilyName, out uint outLength);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowIcon(IntPtr hWnd, int flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetPropA(IntPtr hwnd, string name);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsCallback lpEnumFunc, IntPtr lParam);
        public delegate bool EnumWindowsCallback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public delegate IntPtr ShellProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, ShellProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsTopLevelWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll")]
        public static extern bool SetEvent(IntPtr hEvent);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfoA(uint uiAction, uint uiParam, ref NativeRect pvParam, uint fWinIni);

        public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData);

        [DllImport("User32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        [DllImport("User32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")] // 32-bit Eq. of SetWindowLongPtr
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint SendMessageTimeout(IntPtr hWnd, uint messageId, uint wparam, uint lparam, uint timeoutFlags, uint timeout, ref IntPtr retval);

        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool DeregisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        public const uint ProcessQueryLimitedInformation = 0x1000;

        // DWM API attrib
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }
        // Copied from dwmapi.h
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }
        // DwmSetWindowAttribute
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

        // Proc info API for unpackaged
        [DllImport("kernel32.dll")]
        public static extern void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("shell32.dll", SetLastError = true, EntryPoint = "#188")]
        public static extern bool ShellDDEInit(bool init);

        [DllImport("shell32.dll", SetLastError = true, EntryPoint = "#181")]
        public static extern bool RegisterShellHook(IntPtr hwnd, int fInstall);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        // APPBAR code
        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        public enum ABMsg : int
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

        public enum ABNotify : int
        {
            ABN_STATECHANGE = 0,
            ABN_POSCHANGED,
            ABN_FULLSCREENAPP,
            ABN_WINDOWARRANGE
        }

        public enum ABEdge : int
        {
            ABE_LEFT = 0,
            ABE_TOP,
            ABE_RIGHT,
            ABE_BOTTOM
        }

        public enum ABState
        {
            ABS_TOP = 0x00,
            ABS_AUTOHIDE = 0x01
        }

        public delegate IntPtr WndProcDelegate(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll")]
        public static extern int SHGetPropertyStoreForWindow(IntPtr hwnd, ref Guid iid, [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyStore propertyStore);

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string msg);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public enum SWPFlags
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

        public enum WindowZOrder
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2,
        }

        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();


        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PACKAGE_INFO
        {
            public int reserved;
            public int flags;
            public IntPtr path;
            public IntPtr packageFullName;
            public IntPtr packageFamilyName;
            public PACKAGE_ID packageId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PACKAGE_ID
        {
            public int reserved;
            public AppxPackageArchitecture processorArchitecture;
            public IntPtr version;
            public IntPtr name;
            public IntPtr publisher;
            public IntPtr resourceId;
            public IntPtr publisherId;
        }
        public enum AppxPackageArchitecture
        {
            x86 = 0,
            Arm = 5,
            x64 = 9,
            Neutral = 11,
            Arm64 = 12
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetApplicationUserModelId(IntPtr hProcess, ref int applicationUserModelIdLength, StringBuilder applicationUserModelId);

        public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowProc lpEnumFunc, IntPtr lParam);

        [DllImport("psapi.dll")]
        public static extern uint GetProcessImageFileName(
            IntPtr hProcess,
            [Out] StringBuilder lpImageFileName,
            [In][MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern ulong GetPackageId(IntPtr hProcess, ref UInt32 bufferLength, IntPtr buffer);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetPackagePath(
            ref PACKAGE_ID packageId,
            uint reserved,
            ref uint pathLength,
            IntPtr path);

        [DllImport("ext-ms-win-ntuser-window-l1-1-4.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fUnknown);
    }
}