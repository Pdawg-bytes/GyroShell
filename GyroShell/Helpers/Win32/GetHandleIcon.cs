using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Helpers.Win32
{
    internal class GetHandleIcon
    {
        internal static Icon GetIcon(IntPtr hwnd)
        {
            GetWindowThreadProcessId(hwnd, out uint pid);
            Process proc = Process.GetProcessById((int)pid);
            return Icon.ExtractAssociatedIcon(proc.MainModule.FileName);
        }
        public static SoftwareBitmapSource GetWinUI3BitmapSourceFromHIcon(Icon icon)
        {
            if (icon == null)
                return null;

            // convert to bitmap
            using var bmp = icon.ToBitmap();
            return GetWinUI3BitmapSourceFromGdiBitmap(bmp);
        }
        public static SoftwareBitmapSource GetWinUI3BitmapSourceFromGdiBitmap(Bitmap bmp)
        {
            if (bmp == null)
                return null;

            // get pixels as an array of bytes
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            var bytes = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            bmp.UnlockBits(data);

            // get WinRT SoftwareBitmap
            var softwareBitmap = new SoftwareBitmap(
                Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                bmp.Width,
                bmp.Height,
                Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied);
            softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

            // build WinUI3 SoftwareBitmapSource
            var source = new SoftwareBitmapSource();
            source.SetBitmapAsync(softwareBitmap);
            return source;
        }
    }
}
