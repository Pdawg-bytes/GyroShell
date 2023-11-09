using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Library.Events
{
    public class ShellHookEventArgs : EventArgs
    {
        public enum ShellHookCode
        {
            WindowActivated = HSHELL_WINDOWACTIVATED,
            WindowReplacing = HSHELL_WINDOWREPLACING,
            WindowReplaced = HSHELL_WINDOWREPLACED,
            WindowCreated = HSHELL_WINDOWCREATED,
            WindowDestroyed = HSHELL_WINDOWDESTROYED,
            AppCommand = HSHELL_APPCOMMAND
        }

        /// <summary>
        /// The event arguments used to derive events sent by Windows' ShellHook.
        /// </summary>
        /// <param name="shellMessage">The code of the message being sent.</param>
        /// <param name="objectHandle">The handle of the object that the message is sent for.</param>
        public ShellHookEventArgs(ShellHookCode shellMessage, IntPtr objectHandle)
        {
            ShellMessage = shellMessage;
            ObjectHandle = objectHandle;
        }

        public ShellHookCode ShellMessage { get; set; }

        public IntPtr ObjectHandle { get; set; }
    }
}
