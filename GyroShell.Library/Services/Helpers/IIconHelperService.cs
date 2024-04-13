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

using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Helpers
{
    public interface IIconHelperService
    {
        /// <summary>
        /// Checks if the target window is an UWP application.
        /// </summary>
        /// <param name="hWnd">The window handle of the target app.</param>
        /// <returns><see langword="true"/> if the target window is an UWP app.</returns>
        public bool IsUwpWindow(IntPtr hWnd);

        /// <summary>
        /// Gets the app icon regardless if its a Win32 or UWP window.
        /// </summary>
        /// <param name="hwnd">The handle of the target window.</param>
        /// <param name="targetSize">The target size of the returned icon.</param>
        /// <returns>A <see cref="SoftwareBitmapSource"/> containing the window's icon.</returns>
        public SoftwareBitmapSource GetUwpOrWin32Icon(IntPtr hWnd, int targetSize);
    }
}
