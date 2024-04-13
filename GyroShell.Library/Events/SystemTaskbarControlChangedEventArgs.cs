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
