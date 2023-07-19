using System;
using System.IO;
using System.Text;
using Windows.Storage;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.ApplicationModel;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Media.Imaging;

using static GyroShell.Interfaces.AUMIDIPropertyStore;
using static GyroShell.Helpers.Win32.Win32Interop;
using Windows.Management.Deployment;

namespace GyroShell.Helpers.WinRT
{
    internal class UWPWindowHelper
    {
        public static string GetUwpAppIconPath(IntPtr hwnd)
        {
            return GetPackageFromAppHandle(hwnd).Logo.AbsolutePath;
        }

        public static async Task<SoftwareBitmap> LoadSoftwareBitmapFromUwpIcon(string filePath)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || softwareBitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
                {
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                return softwareBitmap;
            }
        }

        public static SoftwareBitmapSource ConvertSoftwareBitmapToSoftwareBitmapSource(SoftwareBitmap softwareBitmap)
        {
            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            bitmapSource.SetBitmapAsync(softwareBitmap);
            return bitmapSource;
        }

        public static bool IsUwpWindow(IntPtr hWnd)
        {
            return true;
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetPropertyStoreForWindow(IntPtr hwnd, ref Guid iid, [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyStore propertyStore);


        public static Package GetPackageFromAppHandle(IntPtr appHandle)
        {
            // Get the AUMID associated with the app handle
            Guid guidPropertyStore = new Guid("{886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99}");
            IPropertyStore propertyStore;
            int result = SHGetPropertyStoreForWindow(appHandle, ref guidPropertyStore, out propertyStore);
            if (result != 0)
            {
                throw new InvalidOperationException("[-] Failed to retrieve the property store.");
            }

            // Get the AUMID value from the property store
            PropertyKey propertyKey = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
            PropVariant value = new PropVariant();
            propertyStore.GetValue(ref propertyKey, out value);
            string aumid = null;

            try
            {
                if (value.VarType == (ushort)VarEnum.VT_LPWSTR)
                {
                    aumid = Marshal.PtrToStringUni(value.Data);
                }
            }
            finally
            {
                value.Dispose();
            }

            if (string.IsNullOrEmpty(aumid))
            {
                throw new InvalidOperationException("[-] Failed to retrieve the AUMID.");
            }

            // Get the Package object from the AUMID
            try
            {
                // Fails with ACCESS_DENIED.
                PackageManager packageManager = new PackageManager();
                var packages = packageManager.FindPackages();

                foreach (var package in packages)
                {
                    if (package.Id.FamilyName == aumid)
                    {
                        return package;
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("UWPWindowHelper => AUMID Filter: " + e.Message);
            }

            return null;
        }
    }
}
