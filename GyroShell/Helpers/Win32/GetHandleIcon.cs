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

namespace GyroShell.Helpers.Win32
{
    internal class GetHandleIcon
    {
        internal static SoftwareBitmapSource CheckIcon(IntPtr hwnd, int targetSize)
        {
            if (IsUwpWindow(hwnd))
            {
                return GetUWPBitmapSourceFromHwnd(hwnd);
            }
            else
            {
                return GetWinUI3BitmapSourceFromHIcon(GetIcon(hwnd, 32));
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

        internal static SoftwareBitmapSource GetUWPBitmapSourceFromHwnd(IntPtr hWnd)
        {
            try
            {
                string iconPath = GetUwpAppIconPath(hWnd);
                SoftwareBitmap bmp = LoadSoftwareBitmapFromUwpIcon(iconPath).Result;
                return ConvertSoftwareBitmapToSoftwareBitmapSource(bmp);
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
                // Get pixels as an array of bytes from the smoothed bitmap
                BitmapData data = smoothedBmp.LockBits(new Rectangle(0, 0, smoothedBmp.Width, smoothedBmp.Height), ImageLockMode.ReadOnly, smoothedBmp.PixelFormat);
                byte[] bytes = new byte[data.Stride * data.Height];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                smoothedBmp.UnlockBits(data);

                // Get WinRT SoftwareBitmap
                SoftwareBitmap softwareBitmap = new SoftwareBitmap(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, smoothedBmp.Width, smoothedBmp.Height, Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied);
                softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

                // Build WinUI3 SoftwareBitmapSource
                SoftwareBitmapSource source = new SoftwareBitmapSource();
                source.SetBitmapAsync(softwareBitmap);
                return source;
            }
        }

        private static Bitmap ApplyBicubicInterpolation(Bitmap source, int targetWidth, int targetHeight)
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
