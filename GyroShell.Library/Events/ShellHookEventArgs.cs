#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Library.Events
{
    public class ShellHookEventArgs : EventArgs
    {
        /// <summary>
        /// Abstraction of neccessary shell message codes.
        /// </summary>
        public enum ShellHookCode
        {
            ObjectCreated = EVENT_OBJECT_CREATE,
            ObjectDestroyed = EVENT_OBJECT_DESTROY,
            ObjectNamechange = EVENT_OBJECT_NAMECHANGED,
            ForegroundChange = EVENT_SYSTEM_FOREGROUND,
            ObjectCloaked = EVENT_OBJECT_CLOAKED,
            ObjectUncloaked = EVENT_OBJECT_UNCLOAKED
        }

        /// <summary>
        /// The event arguments used to derive events sent by Windows' EventHook.
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
