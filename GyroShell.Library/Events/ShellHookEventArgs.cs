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
