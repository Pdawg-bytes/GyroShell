#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using System.Drawing;

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
