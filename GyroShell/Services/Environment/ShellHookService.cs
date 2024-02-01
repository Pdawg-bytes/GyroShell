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
        private readonly int _wmShellHook;

        private WndProcDelegate _procedureDelegate = null;
        private IntPtr _oldWndProc;

        public IntPtr MainWindowHandle { get; init; }

        public ShellHookService() 
        {
            _oldWndProc = SetWndProc(WindowProcedureCallback);

            _wmShellHook = RegisterWindowMessage("SHELLHOOK");
            RegisterShellHook(MainWindowHandle, 3);
            ShellDDEInit(true);
        }

        private IntPtr SetWndProc(WndProcDelegate procDelegate)
        {
            _procedureDelegate = procDelegate;
            IntPtr wndProcPtr = Marshal.GetFunctionPointerForDelegate(_procedureDelegate);
            if (IntPtr.Size == 8) { return SetWindowLongPtr(MainWindowHandle, GWLP_WNDPROC, wndProcPtr); }
            else { return SetWindowLong(MainWindowHandle, GWLP_WNDPROC, wndProcPtr); }
        }
        private IntPtr WindowProcedureCallback(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam)
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

            }
            return IntPtr.Zero;
        }
    }
}
