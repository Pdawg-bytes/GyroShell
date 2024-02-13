using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Helpers
{
    /// <summary>
    /// Defines a service interface for utility functions regarding bitmaps.
    /// </summary>
    public interface IBitmapHelperService
    {
        /// <summary>
        /// Removes the transparent padding around a bitmap.
        /// </summary>
        /// <param name="bmp">The target <see cref="Bitmap"/>.</param>
        /// <returns>The modified <see cref="Bitmap"/>.</returns>
        public Bitmap RemoveTransparentPadding(Bitmap bmp);

        /// <summary>
        /// Applies bicubic filtering and optionally scales a bitmap.
        /// </summary>
        /// <param name="source">The target <see cref="Bitmap"/>.</param>
        /// <param name="targetWidth">The target width.</param>
        /// <param name="targetHeight">The target height.</param>
        /// <returns>The modified <see cref="Bitmap"/>.</returns>
        public Bitmap FilterAndScaleBitmap(Bitmap source, int targetWidth, int targetHeight);

        /// <summary>
        /// Loads a bitmap from a file path.
        /// </summary>
        /// <param name="filePath">The file path to use.</param>
        /// <returns>The loaded <see cref="Bitmap"/>.</returns>
        public Bitmap LoadBitmapFromPath(string filePath);
    }
}
