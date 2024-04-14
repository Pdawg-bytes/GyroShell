#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
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
