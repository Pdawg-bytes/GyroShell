using System;
using System.Drawing;
using Windows.ApplicationModel;
using Windows.Storage.Streams;

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
        /// Gets the AUMID package from an UWP window handle.
        /// </summary>
        /// <param name="hWnd">The window handle of the target UWP app.</param>
        /// <returns>The app's <see cref="Package"/> object.</returns>
        public Package GetPackageFromAppHandle(IntPtr hWnd);

        /// <summary>
        /// Gets the window title of a window.
        /// </summary>
        /// <param name="hWnd">The handle of the target window.</param>
        /// <returns>The window title.</returns>
        public string GetWindowTitle(IntPtr hWnd);

        /// <summary>
        /// Gets a stream reference to a UWP app's icon.
        /// </summary>
        /// <param name="hWnd">The window handle of the target UWP app.</param>
        /// <returns>A <see cref="RandomAccessStreamReference"/> containing the apps' icon.</returns>
        public RandomAccessStreamReference GetUwpIconStream(IntPtr hWnd);
    }
}