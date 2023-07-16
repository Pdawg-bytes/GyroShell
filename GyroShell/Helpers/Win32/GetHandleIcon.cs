using System.Drawing;
using System;
using static GyroShell.Helpers.Win32.Win32Interop;
using System.Runtime.InteropServices;
using Windows.UI.Core;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.IO;

namespace GyroShell.Helpers.Win32
{
    internal class GetHandleIcon
    {
        internal static IntPtr GetIcon(IntPtr hwnd)
        {
            IntPtr icon = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);
            if (icon == IntPtr.Zero)
                icon = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
            if (icon == IntPtr.Zero)
                icon = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
            if (icon == IntPtr.Zero)
                icon = GetClassLongPtr(hwnd, -14);
            if (icon == IntPtr.Zero)
                icon = GetClassLongPtr(hwnd, -34);

            return icon;
        }
    }
}
