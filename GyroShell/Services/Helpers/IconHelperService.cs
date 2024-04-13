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
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

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

        public SoftwareBitmapSource GetUwpOrWin32Icon(IntPtr hWnd, int targetSize)
        {
            if (IsUwpWindow(hWnd))
            {
                return ConvertIconBitmapToSoftwareBitmapSource(GetUWPBitmap(hWnd));
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
                GetWindowThreadProcessId(hWnd, out uint pid);
                Process proc = Process.GetProcessById((int)pid);
                Icon icon = Icon.ExtractAssociatedIcon(proc.MainModule.FileName);

                if (icon != null)
                {
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
        private Bitmap GetUWPBitmap(IntPtr hWnd)
        {
            try
            {
                string iconPath = m_appHelper.GetUwpAppIconPath(hWnd);
                return (Bitmap)Image.FromFile(iconPath);

                /*using (IRandomAccessStream stream = await m_appHelper.GetUwpIconStream(hWnd).OpenReadAsync())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    Bitmap bitmap;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memoryStream.AsRandomAccessStream());
                        encoder.SetSoftwareBitmap(softwareBitmap);
                        await encoder.FlushAsync();

                        memoryStream.Seek(0, SeekOrigin.Begin);

                        using (MemoryStream copiedStream = new MemoryStream())
                        {
                            await memoryStream.CopyToAsync(copiedStream);
                            copiedStream.Seek(0, SeekOrigin.Begin);
                            bitmap = new Bitmap(copiedStream);
                        }
                    }

                    return bitmap;
                }*/
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
            using (Bitmap croppedBmp = m_bmpHelper.RemoveTransparentPadding(bmp))
            {
                Bitmap resampledBmp = m_bmpHelper.FilterAndScaleBitmap(croppedBmp, croppedBmp.Width, croppedBmp.Height);

                BitmapData data = resampledBmp.LockBits(new Rectangle(0, 0, resampledBmp.Width, resampledBmp.Height), ImageLockMode.ReadOnly, resampledBmp.PixelFormat);
                byte[] bytes = new byte[data.Stride * data.Height];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                resampledBmp.UnlockBits(data);

                SoftwareBitmap softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, resampledBmp.Width, resampledBmp.Height, BitmapAlphaMode.Premultiplied);
                softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

                SoftwareBitmapSource source = new SoftwareBitmapSource();
                source.SetBitmapAsync(softwareBitmap);
                return source;
            }
        }
        #endregion
    }
}