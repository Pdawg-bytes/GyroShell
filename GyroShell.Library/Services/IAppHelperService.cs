using System;
using System.Drawing;
using Windows.ApplicationModel;

namespace GyroShell.Library.Services
{
    public interface IAppHelperService
    {
        public string GetUwpAppIconPath(IntPtr hWnd);
        public string GetUwpExtraIcons(string path, string appName, string normalPath);

        public bool IsUwpWindow(IntPtr hWnd);
        public Package GetPackageFromAppHandle(IntPtr hWnd);

        public Bitmap GetUwpOrWin32Icon(IntPtr hwnd, int targetSize);
        public string GetWindowTitle(IntPtr hWnd);
    }
}
