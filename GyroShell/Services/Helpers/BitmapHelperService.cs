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
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;

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

        public WriteableBitmap RemoveTransparentPadding(WriteableBitmap bmp)
        {
            Rectangle bounds = GetNonTransparentBounds(bmp);

            if (bounds.IsEmpty)
            {
                return null;
            }

            int croppedWidth = (int)bounds.Width;
            int croppedHeight = (int)bounds.Height;
            WriteableBitmap croppedBitmap = new WriteableBitmap(croppedWidth, croppedHeight);

            byte[] pixels = bmp.PixelBuffer.ToArray();
            byte[] croppedPixels = new byte[croppedWidth * croppedHeight * 4];

            for (int y = 0; y < croppedHeight; y++)
            {
                for (int x = 0; x < croppedWidth; x++)
                {
                    int srcIndex = ((bounds.Top + y) * bmp.PixelWidth + bounds.Left + x) * 4;
                    int destIndex = (y * croppedWidth + x) * 4;

                    croppedPixels[destIndex] = pixels[srcIndex];       // Blue
                    croppedPixels[destIndex + 1] = pixels[srcIndex + 1]; // Green
                    croppedPixels[destIndex + 2] = pixels[srcIndex + 2]; // Red
                    croppedPixels[destIndex + 3] = pixels[srcIndex + 3]; // Alpha
                }
            }
            using (var stream = croppedBitmap.PixelBuffer.AsStream())
            {
                stream.Write(croppedPixels, 0, croppedPixels.Length);
            }

            return croppedBitmap;
        }
        private Rectangle GetNonTransparentBounds(WriteableBitmap bmp)
        {
            if (bmp == null) return Rectangle.Empty;

            byte[] pixels = bmp.PixelBuffer.ToArray();
            int left = bmp.PixelWidth, right = 0, top = bmp.PixelHeight, bottom = 0;

            for (int y = 0; y < bmp.PixelHeight; y++)
            {
                for (int x = 0; x < bmp.PixelWidth; x++)
                {
                    int pixelIndex = (y * bmp.PixelWidth + x) * 4;
                    byte alpha = pixels[pixelIndex + 3];

                    if (alpha != 0)
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
