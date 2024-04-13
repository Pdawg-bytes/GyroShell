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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Environment
{
    /// <summary>
    /// Defines a list of entry points into GyroShell's internal backend.
    /// </summary>
    public interface IInternalLauncher
    {
        /// <summary>
        /// Launches the shell settings.
        /// </summary>
        public void LaunchShellSettings();

        /// <summary>
        /// Starts a new instance of GyroShell.
        /// </summary>
        /// <remarks>Kills the current instace. The new instance replaces the current one.</remarks>
        public void LaunchNewShellInstance();

        /// <summary>
        /// Exits the current instance of GyroShell (App.Current).
        /// </summary>
        public void ExitGyroShell();

        /// <summary>
        /// An abstraction of GyroShell's public ProcessStartEx method.
        /// </summary>
        /// <param name="procName">The name of the process to launch.</param>
        /// <param name="createNoWindow">Create no window flag.</param>
        /// <param name="useShellEx">Launches the process with ShellExecute.</param>
        public void LaunchProcess(string procName, bool createNoWindow, bool useShellEx);
    }
}
