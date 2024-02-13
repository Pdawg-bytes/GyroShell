using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Helpers
{
    public interface IIconHelperService
    {
        /// <summary>
        /// Checks if the target window is an UWP application.
        /// </summary>
        /// <param name="hWnd">The window handle of the target app.</param>
        /// <returns><see langword="true"/> if the target window is an UWP app.</returns>
        public bool IsUwpWindow(IntPtr hWnd);

        /// <summary>
        /// Gets the app icon regardless if its a Win32 or UWP window.
        /// </summary>
        /// <param name="hwnd">The handle of the target window.</param>
        /// <param name="targetSize">The target size of the returned icon.</param>
        /// <returns>A <see cref="SoftwareBitmapSource"/> containing the window's icon.</returns>
        public SoftwareBitmapSource GetUwpOrWin32Icon(IntPtr hWnd, int targetSize);
    }
}
