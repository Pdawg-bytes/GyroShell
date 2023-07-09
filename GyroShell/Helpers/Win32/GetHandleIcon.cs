using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Helpers.Win32
{
    internal class GetHandleIcon
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport("user32.dll")]
        private static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines,
            byte[] lpvBits, ref BITMAPINFO lpbi, uint uUsage);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern int GetObject(IntPtr hObject, int nSize, ref BITMAP bm);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public IntPtr bmBits;
        }

        private struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public int bmiColors;
        }

        private struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        private static async Task<WriteableBitmap> HIconToWriteableBitmap(IntPtr hIcon)
        {
            ICONINFO iconInfo;
            GetIconInfo(hIcon, out iconInfo);

            IntPtr dc = GetDC(IntPtr.Zero);

            BITMAP bm = new BITMAP();
            GetObject(iconInfo.hbmColor, Marshal.SizeOf(typeof(BITMAP)), ref bm);

            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            bmi.bmiHeader.biWidth = bm.bmWidth;
            bmi.bmiHeader.biHeight = -bm.bmHeight;
            bmi.bmiHeader.biPlanes = 1;
            bmi.bmiHeader.biBitCount = 32;
            bmi.bmiHeader.biCompression = 0;

            int nBits = bm.bmWidth * bm.bmHeight;
            byte[] colorBits = new byte[nBits * 4];

            if (GetDIBits(dc, iconInfo.hbmColor, 0, (uint)bm.bmHeight, colorBits, ref bmi, 0) == 0)
            {
                ReleaseDC(IntPtr.Zero, dc);
                DeleteObject(iconInfo.hbmColor);
                DeleteObject(iconInfo.hbmMask);
                DestroyIcon(hIcon);
                return null;
            }

            bool hasAlpha = false;
            for (int i = 3; i < nBits * 4; i += 4)
            {
                if (colorBits[i] != 0)
                {
                    hasAlpha = true;
                    break;
                }
            }

            if (!hasAlpha)
            {
                byte[] maskBits = new byte[nBits * 4];
                if (GetDIBits(dc, iconInfo.hbmMask, 0, (uint)bm.bmHeight, maskBits, ref bmi, 0) == 0)
                {
                    ReleaseDC(IntPtr.Zero, dc);
                    DeleteObject(iconInfo.hbmColor);
                    DeleteObject(iconInfo.hbmMask);
                    DestroyIcon(hIcon);
                    return null;
                }

                for (int i = 3; i < nBits * 4; i += 4)
                {
                    if (maskBits[i] == 0)
                    {
                        colorBits[i] = 255;
                    }
                }

                maskBits = null;
            }

            ReleaseDC(IntPtr.Zero, dc);
            DeleteObject(iconInfo.hbmColor);
            DeleteObject(iconInfo.hbmMask);
            DestroyIcon(hIcon);

            WriteableBitmap bitmap = new WriteableBitmap(bm.bmWidth, bm.bmHeight);
            using (Stream stream = bitmap.PixelBuffer.AsStream())
            {
                IBuffer buffer = colorBits.AsBuffer();
                await stream.WriteAsync(buffer.ToArray(), 0, (int)buffer.Length);
            }

            return bitmap;
        }

        internal static WriteableBitmap GetAppIcon(IntPtr hwnd)
        {
            IntPtr iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICON);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);

            if (iconHandle == IntPtr.Zero)
                return null;

            return HIconToWriteableBitmap(iconHandle).Result;
        }
    }
}
