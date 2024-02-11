﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GyroShell.Library.Events;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Helpers;
using Microsoft.UI.Xaml.Media.Imaging;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using static GyroShell.Library.Helpers.Win32.WindowChecks;
using static GyroShell.Library.Models.InternalData.IconModel;

namespace GyroShell.Services.Environment
{
    public class ShellHookService : IShellHookService
    {
        private readonly IBitmapHelperService m_bmpHelper;
        private readonly IAppHelperService m_appHelper;

        private int _wmShellHook;

        private WndProcDelegate _procedureDelegate = null;
        private IntPtr _oldWndProc;

        public IntPtr MainWindowHandle { get; set; }

        private ObservableCollection<IconModel> _currentWindows;

        public ShellHookService(IBitmapHelperService bmpHelper, IAppHelperService appHelper) 
        {
            m_bmpHelper = bmpHelper;
            m_appHelper = appHelper;

            _currentWindows = new ObservableCollection<IconModel>();
        }

        public void Initialize()
        {
            _oldWndProc = SetWndProc(WndProcCallback);

            EnumWindows(EnumWindowsCallback, IntPtr.Zero);

            _wmShellHook = RegisterWindowMessage("SHELLHOOK");
            RegisterShellHook(MainWindowHandle, 3);
            ShellDDEInit(true);
        }

        private bool EnumWindowsCallback(IntPtr hwnd, IntPtr lParam)
        {
            try
            {
                if (IsUserWindow(hwnd))
                {
                    _currentWindows.Add(new IconModel { IconName = m_appHelper.GetWindowTitle(hwnd), Id = hwnd, AppIcon = m_bmpHelper.GetXamlBitmapFromGdiBitmapAsync(m_appHelper.GetUwpOrWin32Icon(hwnd, 32)).Result });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return true;
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

        private IntPtr ShellHookCallback(int message, IntPtr hWnd)
        {
            switch (message)
            {
                case HSHELL_WINDOWCREATED:
                    if (!_currentWindows.Any(win => win.Id == hWnd))
                    {
                        AddWindow(hWnd);
                    }
                    break;
                case HSHELL_WINDOWDESTROYED:
                    RemoveWindow(hWnd);
                    break;
                case HSHELL_WINDOWREPLACED:
                    RemoveWindow(hWnd);
                    break;
                case HSHELL_WINDOWACTIVATED:
                case HSHELL_RUDEAPPACTIVATED:
                    foreach (IconModel win in _currentWindows.Where(w => w.State == WindowState.Active))
                    {
                        win.State = WindowState.Inactive;
                    }

                    IconModel model = null;
                    if (_currentWindows.Any(w => w.Id == hWnd))
                    {
                        model = _currentWindows.First(w => w.Id == hWnd);
                        model.State = WindowState.Active;
                    }
                    else
                    {
                        AddWindow(hWnd);
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void AddWindow(IntPtr hWnd)
        {
            if (IsUserWindow(hWnd))
            {
                _currentWindows.Add(CreateNewIcon(hWnd));
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

        private IconModel CreateNewIcon(IntPtr hWnd)
        {
            Bitmap icon = m_appHelper.GetUwpOrWin32Icon(hWnd, 23);
            SoftwareBitmapSource bmp = m_bmpHelper.GetXamlBitmapFromGdiBitmapAsync(icon).Result;
            string windowName = m_appHelper.GetWindowTitle(hWnd);
            return new IconModel { IconName = windowName, Id = hWnd, AppIcon = bmp };
        }


        public ObservableCollection<IconModel> CurrentWindows
        {
            get => _currentWindows;
        }
    }
}