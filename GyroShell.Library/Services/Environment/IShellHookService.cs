#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Models.InternalData;
using System;
using System.Collections.ObjectModel;

namespace GyroShell.Library.Services.Environment
{
    public interface IShellHookService
    {
        public void Initialize();

        public IntPtr MainWindowHandle { get; set; }

        public ObservableCollection<WindowModel> CurrentWindows { get; }
    }
}
