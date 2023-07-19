using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices;
﻿using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

using static GyroShell.Helpers.Win32.Win32Interop;
using static GyroShell.Helpers.WinRT.UWPWindowHelper;
using System.Threading.Tasks;

namespace GyroShell.Helpers.Win32
{
    internal class GetHandleIcon
    {
        internal static SoftwareBitmapSource CheckIcon(IntPtr hwnd, int targetSize)
        {
            if (IsUwpWindow(hwnd))
            {
                return GetUWPBitmapSourceFromHwnd(hwnd).Result;
            }
            else
            {
                return GetWinUI3BitmapSourceFromHIcon(GetIcon(hwnd, targetSize));
            }
        }

        internal static Icon GetIcon(IntPtr hwnd, int targetSize)
        {
            GetWindowThreadProcessId(hwnd, out uint pid);
            Process proc = Process.GetProcessById((int)pid);
            Icon icon = Icon.ExtractAssociatedIcon(proc.MainModule.FileName);

            if (icon != null)
            {
                // Resize the icon while preserving aspect ratio
                Bitmap resizedIcon = new Bitmap(icon.ToBitmap(), new Size(targetSize, targetSize));
                return Icon.FromHandle(resizedIcon.GetHicon());
            }

            return null;
        }

        internal async static Task<SoftwareBitmapSource> GetUWPBitmapSourceFromHwnd(IntPtr hWnd)
        {
            try
            {
                string iconPath = GetUwpAppIconPath(hWnd);
                Bitmap bmp = await LoadBitmapFromUwpIcon(iconPath);
                return ConvertBitmapToSoftwareBitmapSource(bmp).Result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        internal static SoftwareBitmapSource GetWinUI3BitmapSourceFromHIcon(Icon icon)
        {
            if (icon == null)
                return null;

            // Convert to bitmap
            using (Bitmap bmp = icon.ToBitmap())
            {
                return GetWinUI3BitmapSourceFromGdiBitmap(bmp);
            }
        }

        internal static SoftwareBitmapSource GetWinUI3BitmapSourceFromGdiBitmap(Bitmap bmp)
        {
            if (bmp == null)
                return null;

            using (Bitmap smoothedBmp = ApplyBicubicInterpolation(bmp, bmp.Width, bmp.Height))
            {
                BitmapData data = smoothedBmp.LockBits(new Rectangle(0, 0, smoothedBmp.Width, smoothedBmp.Height), ImageLockMode.ReadOnly, smoothedBmp.PixelFormat);
                byte[] bytes = new byte[data.Stride * data.Height];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                smoothedBmp.UnlockBits(data);

                SoftwareBitmap softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, smoothedBmp.Width, smoothedBmp.Height, BitmapAlphaMode.Premultiplied);
                softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

                SoftwareBitmapSource source = new SoftwareBitmapSource();
                source.SetBitmapAsync(softwareBitmap);
                return source;
            }
        }

        internal static Bitmap RemoveTransparentBorders(Bitmap bmp)
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
        private static Rectangle GetNonTransparentBounds(Bitmap bmp)
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

        internal static Bitmap ApplyBicubicInterpolation(Bitmap source, int targetWidth, int targetHeight)
        {
            Bitmap resampledImage = new Bitmap(targetWidth, targetHeight);

            using (Graphics graphics = Graphics.FromImage(resampledImage))
            {
                // Set interpolation and resample
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, new Rectangle(0, 0, targetWidth, targetHeight));
            }

            return resampledImage;
        }
    }
}
