using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GyroShell.Library.Events;
using GyroShell.Library.Services.Environment;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

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

        public ShellHookService()
        {
        }

        private void InitializeHooks()
        {
            _hooksRegistered = true;
            _shellHook = RegisterWindowMessage("SHELLHOOK");
            RegisterShellHook(_mainWindowHandle, 3);

            _oldWndProc = SetWndProc(WindowProcess);

            ShellDDEInit(true);
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
        private IntPtr HandleShellHook(int iCode, IntPtr hwnd)
        {
            ShellHookEventArgs.ShellHookCode sh = (ShellHookEventArgs.ShellHookCode)iCode;
            return IntPtr.Zero;
        }
    }
}
