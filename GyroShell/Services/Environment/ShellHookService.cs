using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GyroShell.Library.Events;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.Services.Environment;
using Microsoft.UI.Xaml.Media.Imaging;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using static GyroShell.Library.Helpers.Win32.WindowChecks;

namespace GyroShell.Services.Environment
{
    public class ShellHookService : IShellHookService
    {
        private bool _hooksRegistered = false;
        private IntPtr _mainWindowHandle;
        public IntPtr MainWindowHandle
        {
            get => _mainWindowHandle;
            set
            {
                _mainWindowHandle = value;
                if (value != IntPtr.Zero && !_hooksRegistered) { InitializeHooks(); }
            }
        }

        private int _shellHook;
            
        private WndProcDelegate _currDelegate = null;
        private IntPtr _oldWndProc;

        private List<IntPtr> _indexedWindows;
        public List<IntPtr> IndexedWindows
        {
            get => _indexedWindows;
            set => _indexedWindows = value;
        }

        public ShellHookService()
        {
            IndexedWindows = new List<IntPtr>();
        }

        private void InitializeHooks()
        {
            _hooksRegistered = true;
            GetCurrentWindows();
            _shellHook = RegisterWindowMessage("SHELLHOOK");
            RegisterShellHook(_mainWindowHandle, 3);

            _oldWndProc = SetWndProc(WindowProcess);

            ShellDDEInit(true);
        }

        private void GetCurrentWindows()
        {
            EnumWindows(EnumWindowsCallbackMethod, IntPtr.Zero);
        }
        private bool EnumWindowsCallbackMethod(IntPtr hwnd, IntPtr lParam)
        {
            try
            {
                if (IsUserWindow(hwnd))
                {
                    HandleShellHook(1, hwnd);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return true;
        }


        private IntPtr SetWndProc(WndProcDelegate newProc)
        {
            _currDelegate = newProc;

            IntPtr newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newProc);

            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr(_mainWindowHandle, GWLP_WNDPROC, newWndProcPtr);
            }
            else
            {
                return SetWindowLong(_mainWindowHandle, GWLP_WNDPROC, newWndProcPtr);
            }
        }
        private IntPtr WindowProcess(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message == _shellHook)
            {
                return HandleShellHook(wParam.ToInt32(), lParam);
            }
            return CallWindowProc(_oldWndProc, hwnd, message, wParam, lParam);
        }


        public event EventHandler<ShellHookEventArgs> ShellHookEvent;
        private IntPtr HandleShellHook(int iCode, IntPtr hWnd)
        {
            // TODO: Implement some WinEventHook stuff for other events not caught in HSHELL.
            try
            {
                switch (iCode)
                {
                    case HSHELL_WINDOWCREATED:
                        IndexedWindows.Add(hWnd);
                        break;
                    case HSHELL_WINDOWDESTROYED:
                        IndexedWindows.Remove(hWnd);
                        break;
                }
                ShellHookEvent?.Invoke(this, new ShellHookEventArgs((ShellHookEventArgs.ShellHookCode)iCode, hWnd));
                return IntPtr.Zero;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }
    }
}
