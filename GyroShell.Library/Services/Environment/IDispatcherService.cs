#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using Microsoft.UI.Dispatching;

namespace GyroShell.Library.Services.Environment
{
    /// <summary>
    /// Provides an abstraction of GyroShell's internal dispatcher.
    /// </summary>
    public interface IDispatcherService
    {
        DispatcherQueue DispatcherQueue { get; init; }
    }
}
