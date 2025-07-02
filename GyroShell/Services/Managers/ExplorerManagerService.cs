#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Services.Managers;
using System;
using System.Runtime.InteropServices;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Services.Managers
{
    internal class ExplorerManagerService : IExplorerManagerService
    {
        public IntPtr _hTaskBar { get; set; }
        public IntPtr _hMultiTaskBar { get; set; }
        public IntPtr _hStartMenu { get; set; }

        public void Initialize()
        {
            _hTaskBar = FindWindow("Shell_TrayWnd", null);
            _hMultiTaskBar = FindWindow("Shell_SecondaryTrayWnd", null);
            _hStartMenu = FindWindowEx(_hStartMenu, IntPtr.Zero, "Button", "Start");

            if (_hStartMenu == IntPtr.Zero)
            {
                _hStartMenu = FindWindow("Button", null);
            }
        }


        public void ShowTaskbar() => SetVisibility(true);
        public void HideTaskbar() => SetVisibility(false);


        public void ToggleStartMenu() => SendMessage(_hTaskBar, WM_SYSCOMMAND, /*SC_TASKLIST*/ 0xF130, 0);

        public void ToggleControlCenter() =>
            ShellExecute(IntPtr.Zero, "open", "ms-actioncenter:controlcenter/&suppressAnimations=false&showFooter=true&allowPageNavigation=true", null, null, 1);

        public void ToggleActionCenter() => 
            ShellExecute(IntPtr.Zero, "open", "ms-actioncenter:", null, null, 1);


        public void ToggleAutoHideExplorer(bool doHide)
        {
            if (doHide)
            {
                // MainBar
                APPBARDATA abd = new APPBARDATA();

                abd.cbSize = Marshal.SizeOf(abd);
                abd.hWnd = _hTaskBar;
                abd.lParam = (IntPtr)ABState.ABS_AUTOHIDE;

                SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abd);
            }
            else
            {
                // MainBar
                APPBARDATA abd = new APPBARDATA();

                abd.cbSize = Marshal.SizeOf(abd);
                abd.hWnd = _hTaskBar;
                abd.lParam = (IntPtr)ABState.ABS_TOP;

                SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abd);

                // MultiBar
                if (_hMultiTaskBar != IntPtr.Zero)
                {
                    APPBARDATA abdM = new APPBARDATA();

                    abd.cbSize = Marshal.SizeOf(abdM);
                    abd.hWnd = _hMultiTaskBar;
                    abd.lParam = (IntPtr)ABState.ABS_TOP;

                    SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abdM);
                }
            }
        }

        public void NotifyWinlogonShowShell()
        {
            IntPtr handle = OpenEvent(0x0002, false, "msgina: ShellReadyEvent");

            if (handle != IntPtr.Zero)
            {
                SetEvent(handle);
                CloseHandle(handle);
            }
        }

        private void SetVisibility(bool isVisible)
        {
            if (!isVisible)
            {
                SetWindowPos(_hTaskBar, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, (int)SWPFlags.SWP_HIDEWINDOW | (int)SWPFlags.SWP_NOMOVE | (int)SWPFlags.SWP_NOSIZE | (int)SWPFlags.SWP_NOACTIVATE);
                SetWindowPos(_hMultiTaskBar, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, (int)SWPFlags.SWP_HIDEWINDOW | (int)SWPFlags.SWP_NOMOVE | (int)SWPFlags.SWP_NOSIZE | (int)SWPFlags.SWP_NOACTIVATE);
            }
            else
            {
                SetWindowPos(_hTaskBar, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 48, 0, 0, (int)SWPFlags.SWP_SHOWWINDOW);
                SetWindowPos(_hMultiTaskBar, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 48, 0, 0, (int)SWPFlags.SWP_SHOWWINDOW);
            }
        }
    }
}