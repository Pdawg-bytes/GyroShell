using System;
using System.Drawing;
using Windows.ApplicationModel;

namespace GyroShell.Library.Services.Helpers
{
    /// <summary>
    /// Defines a mostly WinRT-independent service interface to aid with handling Win32 and UWP applications.
    /// </summary>
    public interface IAppHelperService
    {
        /// <summary>
        /// Gets an UWP app's icon path.
        /// </summary>
        /// <param name="hWnd">The window handle of the target UWP app.</param>
        /// <returns>The app's icon path.</returns>
        public string GetUwpAppIconPath(IntPtr hWnd);

        /// <summary>
        /// Checks if the target window is an UWP application.
        /// </summary>
        /// <param name="hWnd">The window handle of the target app.</param>
        /// <returns><see langword="true"/> if the target window is an UWP app.</returns>
        public bool IsUwpWindow(IntPtr hWnd);

        /// <summary>
        /// Gets the AUMID package from an UWP window handle.
        /// </summary>
        /// <param name="hWnd">The window handle of the target UWP app.</param>
        /// <returns>The app's <see cref="Package"/> object.</returns>
        public Package GetPackageFromAppHandle(IntPtr hWnd);

        /// <summary>
        /// Gets the app icon regardless if its a Win32 or UWP window.
        /// </summary>
        /// <param name="hwnd">The handle of the target window.</param>
        /// <param name="targetSize">The target size of the returned icon.</param>
        /// <returns>A <see cref="Bitmap"/> containing the window's icon.</returns>
        public Bitmap GetUwpOrWin32Icon(IntPtr hwnd, int targetSize);

        /// <summary>
        /// Gets the window title of a window.
        /// </summary>
        /// <param name="hWnd">The handle of the target window.</param>
        /// <returns>The window title.</returns>
        public string GetWindowTitle(IntPtr hWnd);
    }
}
