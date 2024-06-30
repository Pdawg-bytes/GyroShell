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
using System.Drawing.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using System.Threading.Tasks;
using WinRT;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;

namespace GyroShell.Services.Helpers
{
    public class IconHelperService : IIconHelperService
    {
        private readonly IBitmapHelperService m_bmpHelper;
        private readonly IAppHelperService m_appHelper;

        public IconHelperService(IBitmapHelperService bmpHelper, IAppHelperService appHelper)
        {
            m_bmpHelper = bmpHelper;
            m_appHelper = appHelper;
        }


        public bool IsUwpWindow(IntPtr hWnd) =>
            m_appHelper.GetPackageFromAppHandle(hWnd) != null;

        public async Task<ImageSource> GetUwpOrWin32Icon(IntPtr hWnd, int targetSize)
        {
            if (IsUwpWindow(hWnd))
            {
                return m_bmpHelper.RemoveTransparentPadding(await GetUWPBitmap(hWnd));
            }
            else
            {
                return ConvertIconBitmapToSoftwareBitmapSource(GetWin32Bitmap(hWnd, 32));
            }
        }


        #region Win32
        private Bitmap GetWin32Bitmap(IntPtr hWnd, int targetSize)
        {
            try
            {
                // get the icon from the HWND
                IntPtr hIcon = SendMessage(hWnd, WM_GETICON, (IntPtr)ICON_BIG, IntPtr.Zero);
                if (hIcon == IntPtr.Zero)
                    hIcon = SendMessage(hWnd, WM_GETICON, (IntPtr)ICON_SMALL, IntPtr.Zero);
                if (hIcon == IntPtr.Zero)
                    hIcon = SendMessage(hWnd, WM_GETICON, (IntPtr)ICON_SMALL2, IntPtr.Zero);
                if (hIcon == IntPtr.Zero)
                    hIcon = GetClassLongPtr64(hWnd, -14);
                if (hIcon == IntPtr.Zero)
                    hIcon = GetClassLongPtr64(hWnd, -34);

                if (hIcon != IntPtr.Zero)
                {
                    Icon icon = Icon.FromHandle(hIcon);
                    Bitmap resizedBitmap = new Bitmap(icon.ToBitmap(), new Size(targetSize, targetSize));
                    Icon resizedIcon = Icon.FromHandle(resizedBitmap.GetHicon());
                    return resizedIcon.ToBitmap();
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetHandleIcon => GetIcon: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region UWP
        private async Task<WriteableBitmap> GetUWPBitmap(IntPtr hWnd)
        {
            try
            {
                string iconPath = m_appHelper.GetUwpAppIconPath(hWnd);
                WriteableBitmap writeableBitmap;

                // Open the file as a stream
                StorageFile file = await StorageFile.GetFileFromPathAsync(iconPath);
                using IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);

                BitmapImage bi = new(new Uri(iconPath));
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                var transform = new BitmapTransform()
                {
                    InterpolationMode = BitmapInterpolationMode.Fant
                };

                // Get the pixel data with the applied transform
                var pixelData = await decoder.GetPixelDataAsync(
                    decoder.BitmapPixelFormat,
                    BitmapAlphaMode.Premultiplied,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                );

                writeableBitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                using (Stream pixelStream = writeableBitmap.PixelBuffer.AsStream())
                {
                    byte[] pixels = pixelData.DetachPixelData();
                    await pixelStream.WriteAsync(pixels, 0, pixels.Length);
                }
                return writeableBitmap;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        #endregion

        #region Common
        private SoftwareBitmapSource ConvertIconBitmapToSoftwareBitmapSource(Bitmap bmp)
        {
            if (bmp == null) return null;
            Bitmap resampledBmp = m_bmpHelper.FilterAndScaleBitmap(bmp, bmp.Width, bmp.Height);

            BitmapData data = resampledBmp.LockBits(
                new Rectangle(0, 0, resampledBmp.Width, resampledBmp.Height),
                ImageLockMode.ReadOnly,
                resampledBmp.PixelFormat);

            byte[] bytes = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            resampledBmp.UnlockBits(data);

            SoftwareBitmap softwareBitmap = new SoftwareBitmap(
                BitmapPixelFormat.Bgra8,
                resampledBmp.Width,
                resampledBmp.Height,
                BitmapAlphaMode.Premultiplied);
            softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

            SoftwareBitmapSource source = new SoftwareBitmapSource();
            source.SetBitmapAsync(softwareBitmap);

            return source;
        }
        #endregion
    }
}