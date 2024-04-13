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

using GyroShell.Library.Services.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GyroShell.Services.Helpers
{
    internal class BitmapHelperService : IBitmapHelperService
    {
        public Bitmap LoadBitmapFromPath(string filePath)
        {
            try
            {
                return new Bitmap(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(nameof(LoadBitmapFromPath) + " => Get: " + ex.Message);
                return null;
            }
        }

        public Bitmap FilterAndScaleBitmap(Bitmap source, int targetWidth, int targetHeight)
        {
            Bitmap resampledImage = new Bitmap(targetWidth, targetHeight);

            using (Graphics graphics = Graphics.FromImage(resampledImage))
            {
                // Set interpolation and resample
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, new Rectangle(0, 0, targetWidth, targetHeight));
            }

            return resampledImage;
        }

        public Bitmap RemoveTransparentPadding(Bitmap bmp)
        {
            Rectangle bounds = GetNonTransparentBounds(bmp);

            if (bounds.IsEmpty)
            {
                return null;
            }

            Bitmap croppedBitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height), bounds, GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }
        private Rectangle GetNonTransparentBounds(Bitmap bmp)
        {
            int left = bmp.Width, right = 0, top = bmp.Height, bottom = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixelColor = bmp.GetPixel(x, y);
                    if (pixelColor.A != 0)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y < top) top = y;
                        if (y > bottom) bottom = y;
                    }
                }
            }

            if (left > right || top > bottom)
            {
                return Rectangle.Empty;
            }

            return Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
        }
    }
}
