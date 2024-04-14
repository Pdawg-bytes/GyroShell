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

namespace GyroShell.Library.Events
{
    public class SystemTaskbarControlChangedEventArgs : EventArgs
    {
        public enum SystemControlChangedType
        {
            Start,
            ActionCenter,
            SystemControls
        }

        public SystemTaskbarControlChangedEventArgs(SystemControlChangedType type, bool value) 
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// The type of control that was changed on the taskbar.
        /// </summary>
        public SystemControlChangedType Type { get; }

        /// <summary>
        /// The value of the change.
        /// </summary>
        public bool Value { get; }
    }
}
