using GyroShell.Library.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace GyroShell.Services
{
    internal class BitmapHelperService : IBitmapHelperService
    {
        public async Task<SoftwareBitmapSource> GetXamlBitmapFromGdiBitmap(Bitmap bmp)
        {
            using (Bitmap resampledBmp = FilterAndScaleBitmap(bmp, bmp.Width, bmp.Height))
            {
                BitmapData data = resampledBmp.LockBits(new Rectangle(0, 0, resampledBmp.Width, resampledBmp.Height), ImageLockMode.ReadOnly, resampledBmp.PixelFormat);
                byte[] bytes = new byte[data.Stride * data.Height];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                resampledBmp.UnlockBits(data);

                SoftwareBitmap softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, resampledBmp.Width, resampledBmp.Height, BitmapAlphaMode.Premultiplied);
                softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

                SoftwareBitmapSource source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(softwareBitmap);

                return source;
            }
        }

        public async Task<SoftwareBitmapSource> GetXamlBitmapFromGdiIcon(Icon icon)
        {
            using (Bitmap bmp = icon.ToBitmap())
            {
                return await GetXamlBitmapFromGdiBitmap(bmp);
            }
        }

        public Bitmap LoadBitmapFromPath(string filePath)
        {
            try
            {
                return new Bitmap(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(nameof(LoadBitmapFromPath) + " => Get: " + ex.Message);
                return null;
            }
        }

        public Bitmap FilterAndScaleBitmap(Bitmap source, int targetWidth, int targetHeight)
        {
            Bitmap resampledImage = new Bitmap(targetWidth, targetHeight);

            using (Graphics graphics = Graphics.FromImage(resampledImage))
            {
                // Set interpolation and resample
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, new Rectangle(0, 0, targetWidth, targetHeight));
            }

            return resampledImage;
        }

        public Bitmap RemoveTransparentPadding(Bitmap bmp)
        {
            Rectangle bounds = GetNonTransparentBounds(bmp);

            if (bounds.IsEmpty)
            {
                return null;
            }

            Bitmap croppedBitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height), bounds, GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }

        private Rectangle GetNonTransparentBounds(Bitmap bmp)
        {
            int left = bmp.Width, right = 0, top = bmp.Height, bottom = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixelColor = bmp.GetPixel(x, y);
                    if (pixelColor.A != 0)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y < top) top = y;
                        if (y > bottom) bottom = y;
                    }
                }
            }

            if (left > right || top > bottom)
            {
                return Rectangle.Empty;
            }

            return Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
        }
    }
}
