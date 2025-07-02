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

namespace GyroShell.Library.Services.Managers
{
    /// <summary>
    /// Defines a service interface for managing the Windows taskbar.
    /// </summary>
    public interface IExplorerManagerService
    {
        /// <summary>
        /// Initializes the service's public components.
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Shows the taskbar.
        /// </summary>
        public void ShowTaskbar();

        /// <summary>
        /// Hides the taskbar.
        /// </summary>
        public void HideTaskbar();

        /// <summary>
        /// Toggles the Windows start menu.
        /// </summary>
        public void ToggleStartMenu();

        /// <summary>
        /// Toggles the Windows control center.
        /// </summary>
        /// <remarks>Only works under Windows 11.</remarks>
        public void ToggleControlCenter();

        /// <summary>
        /// Toggles the Windows action center.
        /// </summary>
        /// <remarks>Only works under Windows 10.</remarks>
        public void ToggleActionCenter();

        /// <summary>
        /// Toggles auto hiding for the Windows taskbar.
        /// </summary>
        /// <param name="doHide">If the taskbar should auto-hide or not.</param>
        public void ToggleAutoHideExplorer(bool doHide);

        /// <summary>
        /// Notifies winlogon that the shell has already loaded, hiding the login screen.
        /// </summary>
        public void NotifyWinlogonShowShell();

        /// <summary>
        /// The handle of the primary taskbar.
        /// </summary>
        public IntPtr _hTaskBar { get; set; }

        /// <summary>
        /// The handle of the secondary, tertiary, etc taskbar.
        /// </summary>
        public IntPtr _hMultiTaskBar { get; set; }

        /// <summary>
        /// The handle of the start menu.
        /// </summary>
        public IntPtr _hStartMenu { get; set; }
    }
}