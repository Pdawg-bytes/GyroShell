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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using static GyroShell.Library.Helpers.Win32.WindowChecks;
using static GyroShell.Library.Interfaces.IPropertyStoreAUMID;
using static GyroShell.Library.Models.InternalData.WindowModel;

namespace GyroShell.Services.Environment
{
    public class ShellHookService : IShellHookService
    {
        private readonly IAppHelperService _appHelper;
        private readonly IIconHelperService _iconHelper;
        private readonly IExplorerManagerService _explorerManager;

        private int _wmShellHook;

        private WndProcDelegate _procedureDelegate = null;
        private IntPtr _oldWndProc;

        private WinEventDelegate _winEventDelegate;
        private IntPtr _nameChangeHook;
        private IntPtr _cloakUncloakHook;

        public IntPtr MainWindowHandle { get; set; }

        private ObservableCollection<WindowModel> _currentWindows;

        public ShellHookService(
            IAppHelperService appHelper,
            IIconHelperService iconHelper,
            IExplorerManagerService explorerManager)
        {
            _appHelper = appHelper;
            _iconHelper = iconHelper;
            _explorerManager = explorerManager;

            _currentWindows = new ObservableCollection<WindowModel>();
        }

        public void Initialize()
        {
            _oldWndProc = SetWndProc(WndProcCallback);

            EnumWindows(EnumWindowsCallback, IntPtr.Zero);

            _wmShellHook = RegisterWindowMessage("SHELLHOOK");
            RegisterShellHook(MainWindowHandle, 3);

            _winEventDelegate = WinEventCallback;
            _nameChangeHook = SetWinEventHook(EVENT_OBJECT_NAMECHANGED, EVENT_OBJECT_NAMECHANGED, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            _cloakUncloakHook = SetWinEventHook(EVENT_OBJECT_UNCLOAKED, EVENT_OBJECT_UNCLOAKED, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);

            ShellDDEInit(true);
        }

        public void Uninitialize()
        {
            UnhookWinEvent(_nameChangeHook);
            UnhookWinEvent(_cloakUncloakHook);
        }


        private IntPtr SetWndProc(WndProcDelegate procDelegate)
        {
            _procedureDelegate = procDelegate;
            IntPtr wndProcPtr = Marshal.GetFunctionPointerForDelegate(_procedureDelegate);
            if (IntPtr.Size == 8) { return SetWindowLongPtr(MainWindowHandle, GWLP_WNDPROC, wndProcPtr); }
            else { return SetWindowLong(MainWindowHandle, GWLP_WNDPROC, wndProcPtr); }
        }
        private IntPtr WndProcCallback(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message == _wmShellHook)
            {
                return ShellHookCallback(wParam.ToInt32(), lParam);
            }
            return CallWindowProc(_oldWndProc, hwnd, message, wParam, lParam);
        }


        private bool EnumWindowsCallback(IntPtr hwnd, IntPtr lParam)
        {
            try
            {
                if (IsUserWindow(hwnd))
                {
                    WindowState initialState = GetForegroundWindow() == hwnd ? WindowState.Active : WindowState.Inactive;
                    AddWindow(hwnd, false, initialState);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return true;
        }


        private void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            switch (eventType)
            {
                case EVENT_OBJECT_CLOAKED:
                    switch (_appHelper.GetWindowTitle(hwnd))
                    {
                        case "Start":
                            _explorerManager.IsStartMenuOpen = false;
                            break;
                        case "Quick settings":
                            _explorerManager.IsSystemControlsOpen = false;
                            break;
                        case "Windows Shell Experience Host":
                        case "Notification Center":
                            _explorerManager.IsActionCenterOpen = false;
                            break;
                    }
                    break;
                case EVENT_OBJECT_UNCLOAKED:
                    switch (_appHelper.GetWindowTitle(hwnd))
                    {
                        case "Start":
                            _explorerManager.IsStartMenuOpen = true;
                            break;
                        case "Quick settings":
                            _explorerManager.IsSystemControlsOpen = true;
                            break;
                        case "Windows Shell Experience Host":
                        case "Notification Center":
                            _explorerManager.IsActionCenterOpen = true;
                            break;
                    }
                    break;
                case EVENT_OBJECT_NAMECHANGED:
                    if (_currentWindows.Any(wnd => wnd.Id == hwnd))
                    {
                        _currentWindows.First(win => win.Id == hwnd).WindowName = _appHelper.GetWindowTitle(hwnd);
                    }
                    break;
            }
        }


        private IntPtr ShellHookCallback(int message, IntPtr hWnd)
        {
            switch (message)
            {
                case HSHELL_WINDOWCREATED:
                    if (!_currentWindows.Any(win => win.Id == hWnd))
                    {
                        AddWindow(hWnd, false);
                    }
                    break;
                case HSHELL_WINDOWDESTROYED:
                    RemoveWindow(hWnd);
                    break;
                case HSHELL_REDRAW:
                    if (!_currentWindows.Any(win => win.Id == hWnd))
                    {
                        AddWindow(hWnd, IsShellFrameWindow(hWnd));
                    }
                    break;
                case HSHELL_WINDOWREPLACING:
                    if (_currentWindows.Any(wnd => wnd.Id == hWnd))
                    {
                        WindowModel win = _currentWindows.First(i => i.Id == hWnd);
                        win.State = WindowState.Inactive;
                        if (!IsUserWindow(hWnd))
                        {
                            RemoveWindow(hWnd);
                        }
                    }
                    else
                    {
                        AddWindow(hWnd, false);
                    }
                    break;
                case HSHELL_WINDOWREPLACED:
                    RemoveWindow(hWnd);
                    break;
                case HSHELL_WINDOWACTIVATED:
                case HSHELL_RUDEAPPACTIVATED:
                    if (hWnd == IntPtr.Zero) { break; }
                    foreach (WindowModel win in _currentWindows.Where(w => w.State == WindowState.Active))
                    {
                        win.State = WindowState.Inactive;
                    }

                    WindowModel model = null;
                    if (_currentWindows.Any(w => w.Id == hWnd))
                    {
                        model = _currentWindows.First(w => w.Id == hWnd);
                        model.State = WindowState.Active;
                    }
                    else
                    {
                        AddWindow(hWnd, false, WindowState.Active);
                    }
                    break;
                case HSHELL_FLASH:
                    if (_currentWindows.Any(win => win.Id == hWnd))
                    {
                        WindowModel win = _currentWindows.First(wnd => wnd.Id == hWnd);
                        if (win.State != WindowState.Active)
                        {
                            win.State = WindowState.Flashing;
                        }
                    }
                    else
                    {
                        AddWindow(hWnd, false);
                    }
                    break;
                case HSHELL_ENDTASK:
                    RemoveWindow(hWnd);
                    break;
            }
            return IntPtr.Zero;
        }

        private async void AddWindow(IntPtr hWnd, bool checkUwp, WindowState initialState = WindowState.Inactive)
        {
            if (IsUserWindow(hWnd) || checkUwp)
            {
                _currentWindows.Add(await CreateNewIcon(hWnd, initialState));
                //Debug.WriteLine(_appHelper.GetWindowTitle(hWnd) + "; " + hWnd + "; " + _appHelper.GetHandlePath(hWnd));
            }
        }
        private void RemoveWindow(IntPtr hWnd)
        {
            if (_currentWindows.Any(win => win.Id == hWnd))
            {
                do
                {
                    _currentWindows.Remove(_currentWindows.First(wdw => wdw.Id == hWnd));
                }
                while (_currentWindows.Any(win => win.Id == hWnd));
            }
        }

        private async Task<WindowModel> CreateNewIcon(IntPtr hWnd, WindowState initialState)
        {
            ImageSource bmp = await _iconHelper.GetUwpOrWin32Icon(hWnd, 32);
            string windowName = _appHelper.GetWindowTitle(hWnd);
            return new WindowModel { WindowName = windowName, Id = hWnd, AppIcon = bmp, State = initialState };
        }


        public ObservableCollection<WindowModel> CurrentWindows
        {
            get => _currentWindows;
        }
    }
}