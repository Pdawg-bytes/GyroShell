using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing;
using System.Threading.Tasks;

namespace GyroShell.Library.Services
{
    public interface IBitmapHelperService
    {
        public Bitmap RemoveTransparentPadding(Bitmap bmp);
        public Bitmap FilterAndScaleBitmap(Bitmap source, int targetWidth, int targetHeight);
        public Bitmap LoadBitmapFromPath(string filePath);

        public Task<SoftwareBitmapSource> GetXamlBitmapFromGdiBitmap(Bitmap bmp);
        public Task<SoftwareBitmapSource> GetXamlBitmapFromGdiIcon(Icon icon);
    }
}
