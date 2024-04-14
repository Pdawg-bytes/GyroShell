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
using System.Diagnostics;

namespace GyroShell.Helpers
{
    internal class ProcessStart
    {
        internal static ProcessStartInfo ProcessStartEx(string procName, bool createNoWindow, bool useShellEx)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            if (procName != null)
            {
                startInfo.CreateNoWindow = createNoWindow;
                startInfo.UseShellExecute = useShellEx;
                startInfo.FileName = System.IO.Path.Combine(Environment.SystemDirectory, procName);

                return startInfo;
            }
            else
            {
                return null;
            }
        }
    }
}
