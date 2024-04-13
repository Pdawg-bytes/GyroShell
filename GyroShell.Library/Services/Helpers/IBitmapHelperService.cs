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
using System.Drawing;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Helpers
{
    /// <summary>
    /// Defines a service interface for utility functions regarding bitmaps.
    /// </summary>
    public interface IBitmapHelperService
    {
        /// <summary>
        /// Removes the transparent padding around a bitmap.
        /// </summary>
        /// <param name="bmp">The target <see cref="Bitmap"/>.</param>
        /// <returns>The modified <see cref="Bitmap"/>.</returns>
        public Bitmap RemoveTransparentPadding(Bitmap bmp);

        /// <summary>
        /// Applies bicubic filtering and optionally scales a bitmap.
        /// </summary>
        /// <param name="source">The target <see cref="Bitmap"/>.</param>
        /// <param name="targetWidth">The target width.</param>
        /// <param name="targetHeight">The target height.</param>
        /// <returns>The modified <see cref="Bitmap"/>.</returns>
        public Bitmap FilterAndScaleBitmap(Bitmap source, int targetWidth, int targetHeight);

        /// <summary>
        /// Loads a bitmap from a file path.
        /// </summary>
        /// <param name="filePath">The file path to use.</param>
        /// <returns>The loaded <see cref="Bitmap"/>.</returns>
        public Bitmap LoadBitmapFromPath(string filePath);
    }
}
