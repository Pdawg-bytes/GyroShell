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
using Windows.ApplicationModel;
using Windows.Storage.Streams;

namespace GyroShell.Library.Services.Helpers
{
    /// <summary>
    /// Defines a mostly WinRT-independent service interface to aid with handling Win32 and UWP applications.
    /// </summary>
    public interface IAppHelperService
    {
        /// <summary>
        /// Gets an UWP app's icon path.
        /// </summary>
        /// <param name="hWnd">The window handle of the target UWP app.</param>
        /// <returns>The app's icon path.</returns>
        public string GetUwpAppIconPath(IntPtr hWnd);

        /// <summary>
        /// Gets the AUMID package from an UWP window handle.
        /// </summary>
        /// <param name="hWnd">The window handle of the target UWP app.</param>
        /// <returns>The app's <see cref="Package"/> object.</returns>
        public Tuple<string, string> GetPackageFromAppHandle(IntPtr hWnd);

        /// <summary>
        /// Gets the window title of a window.
        /// </summary>
        /// <param name="hWnd">The handle of the target window.</param>
        /// <returns>The window title.</returns>
        public string GetWindowTitle(IntPtr hWnd);

        /// <summary>
        /// Gets the path of the current windows' backing image.
        /// </summary>
        /// <param name="hWnd">The handle to the window.</param>
        /// <returns>The path of the windows' backing image.</returns>
        public string GetHandlePath(IntPtr hWnd);
    }
}