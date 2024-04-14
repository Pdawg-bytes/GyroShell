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

namespace GyroShell.Library.Services.Environment
{
    /// <summary>
    /// Defines a service to provide clock data.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// The formatter string for the clock.
        /// </summary>
        public string ClockFormat { get; set; }

        /// <summary>
        /// The formatter string for the date.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// The event that is raised to update frontend properties.
        /// </summary>
        public event EventHandler UpdateClockBinding;
    }
}
