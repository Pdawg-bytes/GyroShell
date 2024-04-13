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

namespace GyroShell.Library.Services.Hardware
{
    /// <summary>
    /// Defines a platform-agnostic service interface to get sound data.
    /// </summary>
    public interface ISoundService
    {
        /// <summary>
        /// The current volume.
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Checks if the audio is muted.
        /// </summary>
        public bool IsMuted { get; set; }

        /// <summary>
        /// An event raised when the volume is changed, including
        /// when it gets muted.
        /// </summary>
        public event EventHandler OnVolumeChanged;
    }
}
