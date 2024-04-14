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
    /// Defines a platform-agnostic service interface to get environment information.
    /// </summary>
    public interface IEnvironmentInfoService
    {
        /// <summary>
        /// The system's CPU architecture.
        /// </summary>
        public string SystemArchitecture { get; init; }

        /// <summary>
        /// The application's package version.
        /// </summary>
        public Version AppVersion { get; init; }

        /// <summary>
        /// The build date of the application package.
        /// </summary>
        public DateTime AppBuildDate { get; init; }

        /// <summary>
        /// Checks if the application is running under Windows 11.
        /// </summary>
        public bool IsWindows11 { get; }

        /// <summary>
        /// Checks if the system theme is set to dark mode.
        /// </summary>
        public bool IsSystemUsingDarkmode { get; }

        /// <summary>
        /// Gets the principal monitor's width in pixels.
        /// </summary>
        public int MonitorWidth { get; }

        /// <summary>
        /// Gets the principal monitor's height in pixels.
        /// </summary>
        public int MonitorHeight { get; }

        /// <summary>
        /// Gets the handle of the MainWindow.
        /// </summary>
        public IntPtr MainWindowHandle { get; set; }

        /// <summary>
        /// The amount of settings windows currently running.
        /// </summary>
        /// <remarks>
        /// Instance count should never exceed 1. If it does, the internal launcher will not launch another instance.
        /// </remarks>
        public int SettingsInstances { get; set; }
    }
}
