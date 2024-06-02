#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Events;
using GyroShell.Library.Services.Managers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Services.Managers
{
    internal class ExplorerManagerService : IExplorerManagerService
    {
        private const uint EVENT_MODIFY_STATE = 0x0002;

        public IntPtr m_hTaskBar { get; set; }
        public IntPtr m_hMultiTaskBar { get; set; }
        public IntPtr m_hStartMenu { get; set; }

        private static Guid SID_ImmersiveShellHookService = new Guid("4624bd39-5fc3-44a8-a809-163a836e9031");
        private static Guid ImmersiveShellHookServiceInterface = new Guid("914d9b3a-5e53-4e14-bbba-46062acb35a4");
        private static IImmersiveShellHookService? HookService;

        public void Initialize()
        {
            //m_hTaskBar = FindWindowW("Shell_TrayWnd", null);
            //m_hMultiTaskBar = FindWindowW("Shell_SecondaryTrayWnd", null);
            //m_hStartMenu = FindWindowExW(m_hStartMenu, IntPtr.Zero, "Button", "Start");

            if (m_hStartMenu == IntPtr.Zero)
            {
                m_hStartMenu = FindWindowW("Button", null);
            }

            RegisterTaskmanWindow();

            /* In order to allow Immersive processes (packaged apps, uwp apps, etc) to run, we need to use the CImmersiveShellController interface.
             However, that API requires IAM access, which means that the app must be signed by Microsoft and must have the .imsrv PE header section.
             In order to circumvent this, we need to copy runtimebroker.exe to a temp folder which meets those requirements, inject a DLL into it
             to replace WinMain with our own, and then run it. This will allow us to start the Immersive Shell Controller and allow Immersive processes to run.

             Copy runtimebroker.exe to a temp folder, write injected dll, and start it.
             */

            string tempPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Temp", "GyroshellImmersiveUtility");
            string process = Path.Combine(tempPath, "GryoShellImmersiveShell.exe");
            Directory.CreateDirectory(tempPath);

            // todo: remove this hack
            Process.Start("taskkill", "/f /im GryoShellImmersiveShell.exe").WaitForExit();
            Process.Start("taskkill", "/f /im explorer.exe").WaitForExit();

            try
            {
                File.Delete(process);
                File.Delete(Path.Combine(tempPath, "rmclient.dll"));
            }
            catch
            {
                Debugger.Break();
            }

            File.Copy(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "RuntimeBroker.exe"), process, true);
            File.WriteAllBytes(Path.Combine(tempPath, "rmclient.dll"), Properties.Resources.ImmersiveShellHook);

            // start the process
            Process.Start(process);
        }
        private static uint windowMessage;
        delegate nint WndProcDelegate(nint hwnd, uint message, nint wParam, nint lParam);
        private static nint TaskmanWindowProc(nint hwnd, uint message, nint wParam, nint lParam)
        {
            if (message == 1)
            {
                // WM_CREATE
                //if (!SetTaskmanWindow(hwnd))
                //{
                //    throw new Win32Exception();
                //}
                // TODO: This causes an issue??
                windowMessage = RegisterWindowMessageW("SHELLHOOK");
                if (!RegisterShellHookWindow(hwnd))
                {
                    throw new Win32Exception();
                }
            }
            //else if (message == 2)
            //{
            //    // WM_DESTROY
            //    if (GetTaskmanWindow() == hwnd)
            //    {
            //        SetTaskmanWindow(0);
            //    }
            //    DeregisterShellHookWindow(hwnd);
            //}
            //else if (message == windowMessage || message == 0x312) //WM_HOTKEY
            //{
            //    if (HookService == null)
            //    {
            //        var x = (Library.Helpers.Win32.Win32Interop.IServiceProvider)new CImmersiveShell();
            //        if (x.QueryService(ref SID_ImmersiveShellHookService, ref ImmersiveShellHookServiceInterface, out object shellhooksrv) < 0)
            //        {
            //            Console.WriteLine("failed to get the immersive shell hook service");
            //            return 0;
            //        }
            //        else
            //        {
            //            HookService = (IImmersiveShellHookService)shellhooksrv;
            //        }

            //        return 0;
            //    }

            //    // Pass the message to TwinUI to allow UWP apps to work correctly.

            //    bool handle = true;
            //    if (wParam == 12)
            //    {
            //        Console.WriteLine("set window");
            //        HookService.SetTargetWindowForSerialization(lParam);
            //    }
            //    else if (wParam == 0x32)
            //    {
            //        handle = false;
            //    }
            //    if (handle)
            //    {
            //        HookService.PostShellHookMessage(wParam, lParam);
            //    }
            //    return 0;
            //}

            return DefWindowProcW(hwnd, message, wParam, lParam);
        }
        private void RegisterTaskmanWindow()
        {
            WNDCLASSEX progman = WNDCLASSEX.Build();
            progman.style = 8;
            progman.hInstance = Marshal.GetHINSTANCE(typeof(ExplorerManagerService).Module);
            progman.lpszClassName = "TaskmanWndClass";
            //progman.lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProcDelegate)TaskmanWindowProc);

            ushort atom = RegisterClassExW(ref progman);
            if (atom == 0)
            {
                //  throw new Win32Exception();
                return;
            }

            var hwnd = CreateWindowExW(0, progman.lpszClassName, null, 0x82000000, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, Marshal.GetHINSTANCE(typeof(ExplorerManagerService).Module), IntPtr.Zero);

            if (hwnd == IntPtr.Zero)
            {
               // throw new Win32Exception();
            }
        }

        public event EventHandler<SystemTaskbarControlChangedEventArgs> SystemControlStateChanged;

        private bool _isStartMenuOpen;
        public bool IsStartMenuOpen
        {
            get => _isStartMenuOpen;
            set
            {
                _isStartMenuOpen = value;
                SystemControlStateChanged?.Invoke(this, new SystemTaskbarControlChangedEventArgs(SystemTaskbarControlChangedEventArgs.SystemControlChangedType.Start, value));
            }
        }

        private bool _isActionCenterOpen;
        public bool IsActionCenterOpen
        {
            get => _isActionCenterOpen;
            set
            {
                _isActionCenterOpen = value;
                SystemControlStateChanged?.Invoke(this, new SystemTaskbarControlChangedEventArgs(SystemTaskbarControlChangedEventArgs.SystemControlChangedType.ActionCenter, value));
            }
        }

        private bool _isSystemControlsOpen;
        public bool IsSystemControlsOpen
        {
            get => _isSystemControlsOpen;
            set
            {
                _isActionCenterOpen = value;
                SystemControlStateChanged?.Invoke(this, new SystemTaskbarControlChangedEventArgs(SystemTaskbarControlChangedEventArgs.SystemControlChangedType.SystemControls, value));
            }
        }


        public void ShowTaskbar()
        {
            SetVisibility(true);
        }

        public void HideTaskbar()
        {
            SetVisibility(false);
        }

        public void ToggleStartMenu()
        {
            SendMessage(m_hTaskBar, /*WM_SYSCOMMAND*/ 0x0112, (IntPtr) /*SC_TASKLIST*/ 0xF130, (IntPtr)0);
        }

        public void ToggleControlCenter()
        {
            ShellExecute(IntPtr.Zero, "open", "ms-actioncenter:controlcenter/&suppressAnimations=false&showFooter=true&allowPageNavigation=true" /* CNTRLCTR, bool, bool, bool */, null, null, 1);
        }

        public void ToggleActionCenter()
        {
            ShellExecute(IntPtr.Zero, "open", "ms-actioncenter:" /* ACTION CENTER */, null, null, 1);
        }

        public void ToggleAutoHideExplorer(bool doHide)
        {
            if (doHide)
            {
                // MainBar
                APPBARDATA abd = new APPBARDATA();

                abd.cbSize = Marshal.SizeOf(abd);
                abd.hWnd = m_hTaskBar;
                abd.lParam = (IntPtr)ABState.ABS_AUTOHIDE;

                SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abd);
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
                if (m_hMultiTaskBar != IntPtr.Zero)
                {
                    APPBARDATA abdM = new APPBARDATA();

                    abd.cbSize = Marshal.SizeOf(abdM);
                    abd.hWnd = m_hMultiTaskBar;
                    abd.lParam = (IntPtr)ABState.ABS_TOP;

                    SHAppBarMessage((int)ABMsg.ABM_SETSTATE, ref abdM);
                }
            }
        }

        public void NotifyWinlogonShowShell()
        {
            IntPtr handle = OpenEvent(EVENT_MODIFY_STATE, false, "msgina: ShellReadyEvent");

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
                SetWindowPos(m_hTaskBar, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, (int)SWPFlags.SWP_HIDEWINDOW | (int)SWPFlags.SWP_NOMOVE | (int)SWPFlags.SWP_NOSIZE | (int)SWPFlags.SWP_NOACTIVATE);
                SetWindowPos(m_hMultiTaskBar, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0, (int)SWPFlags.SWP_HIDEWINDOW | (int)SWPFlags.SWP_NOMOVE | (int)SWPFlags.SWP_NOSIZE | (int)SWPFlags.SWP_NOACTIVATE);
            }
            else
            {
                SetWindowPos(m_hTaskBar, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 48, 0, 0, (int)SWPFlags.SWP_SHOWWINDOW);
                SetWindowPos(m_hMultiTaskBar, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 48, 0, 0, (int)SWPFlags.SWP_SHOWWINDOW);
            }
        }
    }
}