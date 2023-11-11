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
        private readonly WinEventDelegate _callback;

        private bool _hooksRegistered = false;
        private IntPtr _mainWindowHandle;
        public IntPtr MainWindowHandle { get => _mainWindowHandle; set { _mainWindowHandle = value; if (value != IntPtr.Zero && !_hooksRegistered) { InitializeHooks(); }}}

        private List<IntPtr> _indexedWindows;
        public List<IntPtr> IndexedWindows { get => _indexedWindows; set => _indexedWindows = value; }

        private IntPtr _objectCreateDestroyHook;
        private IntPtr _objectCloakChangeHook;
        private IntPtr _objectNameChangeHook;
        private IntPtr _foregroundChangeHook;

        public ShellHookService()
        {
            IndexedWindows = new List<IntPtr>();
            _callback = WinEventCallback;
        }

        private void InitializeHooks()
        {
            _hooksRegistered = true;
            GetCurrentWindows();

            _foregroundChangeHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            _objectCloakChangeHook = SetWinEventHook(EVENT_OBJECT_CLOAKED, EVENT_OBJECT_UNCLOAKED, IntPtr.Zero, _callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            _objectNameChangeHook = SetWinEventHook(EVENT_OBJECT_NAMECHANGED, EVENT_OBJECT_NAMECHANGED, IntPtr.Zero, _callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            _objectCreateDestroyHook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_DESTROY, IntPtr.Zero, _callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);

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
                    HandleWinEvent(EVENT_OBJECT_CREATE, hwnd);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return true;
        }

        public event EventHandler<ShellHookEventArgs> ShellHookEvent;
        private void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (IsUserWindow(hwnd))
            {
                HandleWinEvent((int)eventType, hwnd);
            }
        }
        private void HandleWinEvent(int eventCode, IntPtr hwnd)
        {
            //ShellHookEvent?.Invoke(this, new ShellHookEventArgs((ShellHookEventArgs.ShellHookCode)eventCode, hwnd));
        }
    }
}
