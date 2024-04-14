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
using GyroShell.Library.Services.Environment;

namespace GyroShell.Services.Environment
{
    public class DispatcherService : IDispatcherService
    {
        public DispatcherService() 
        {
            DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        public DispatcherQueue DispatcherQueue { get; init; }
    }
}
