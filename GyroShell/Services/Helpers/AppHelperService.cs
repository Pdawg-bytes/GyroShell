#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using GyroShell.Library.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using static GyroShell.Library.Interfaces.IPropertyStoreAUMID;

namespace GyroShell.Services.Helpers
{
    internal class AppHelperService : IAppHelperService
    {
        private Dictionary<string, string> m_pkgFamilyMap;
        private PackageManager m_pkgManager;

        public AppHelperService()
        {
            m_pkgFamilyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            m_pkgManager = new PackageManager();

            IEnumerable<Package> packages = m_pkgManager.FindPackagesForUser(null);
            foreach (Package package in packages)
            {
                m_pkgFamilyMap[package.Id.FamilyName] = package.Id.FullName;
            }
        }

        public string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);

            GetWindowText(hWnd, sb, sb.Capacity);

            return sb.ToString();
        }


        public RandomAccessStreamReference GetUwpIconStream(IntPtr hWnd)
        {
            Package pkg = GetPackageFromAppHandle(hWnd);
            AppListEntry entry = pkg.GetAppListEntries().First(ent => ent.DisplayInfo.DisplayName.ToLower().Contains(GetWindowTitle(hWnd).ToLower()));

            if (entry != null)
            {
                return entry.DisplayInfo.GetLogo(new Windows.Foundation.Size(176, 176));
            }
            return null;
        }

        public string GetUwpAppIconPath(IntPtr hWnd)
        {
            string normalPath = Uri.UnescapeDataString(Uri.UnescapeDataString(GetPackageFromAppHandle(hWnd).Logo.AbsolutePath)).Replace("/", "\\");
            string finalPath = GetUwpExtraIcons(normalPath, GetWindowTitle(hWnd));

            return finalPath;
        }
        private string GetUwpExtraIcons(string path, string appName)
        {
            string[] pathParts = path.Split('\\');
            string rootAssetsFolder = string.Join("\\", pathParts.Take(pathParts.Length - 1));

            string[] allFiles = Directory.GetFiles(rootAssetsFolder);
            foreach (string filePath in allFiles)
            {
                if (Path.GetFileName(filePath).Contains("StoreLogo.scale-100"))
                {
                    string e = filePath.Replace(" ", "").ToLower();
                    if (e.Contains(appName.Replace(" ", "").ToLower()))
                    {
                        return filePath;
                    }
                }
            }

            return path;
        }

        // I'm so sorry for this entire thing it's so incredibly cursed
        public Package GetPackageFromAppHandle(IntPtr hWnd)
        {
            Guid guidPropertyStore = new Guid("{886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99}");
            IPropertyStore propertyStore;                      
            int result = SHGetPropertyStoreForWindow(hWnd, ref guidPropertyStore, out propertyStore);
            if (result != 0)
            {
                return null;
            }

            PropertyKey propertyKey = new(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
            PropVariant value = new();
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
                return null;
            }

            try
            {
                string[] aumidParts = aumid.Split('!');
                string packageFamilyName = aumidParts[0];

                if (m_pkgFamilyMap.TryGetValue(packageFamilyName, out string packageFullName))
                {
                    return m_pkgManager.FindPackageForUser(null, packageFullName);
                }
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("UWPWindowHelper => AUMID Filter: " + e.Message);
            }

            return null;
        }
    }
}
