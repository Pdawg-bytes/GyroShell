#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using static GyroShell.Library.Interfaces.IPropertyStoreAUMID;

namespace GyroShell.Services.Helpers
{
    internal class AppHelperService : IAppHelperService
    {
        public AppHelperService()
        {
        }

        public string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);

            GetWindowText(hWnd, sb, sb.Capacity);

            return sb.ToString();
        }

        public string GetUwpAppIconPath(IntPtr hWnd)
        {
            var values = GetPackageFromAppHandle(hWnd);
            string normalPath = Uri.UnescapeDataString(values.Item1);
            string entrypoint = values.Item2;
            string finalPath = GetUwpExtraIcons(normalPath, entrypoint);

            return finalPath;
        }
        private string GetUwpExtraIcons(string path, string entrypoint)
        {
            // read manifest
            string manifestPath = Path.Combine(path, "AppXManifest.xml");
            using FileStream stream = File.Open(manifestPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            XDocument xml = XDocument.Load(stream);
            XNamespace ns = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
            XNamespace uap = "http://schemas.microsoft.com/appx/manifest/uap/windows10";

            /*
            var resScale = xml.Element(ns + "Package")
                                .Element(ns + "Resources")
                                .Elements(ns + "Resource")
                                .FirstOrDefault(e => e.Attribute(uap + "Scale") != null);
            string scale = resScale != null ? ".scale-" + resScale.Attribute(uap + "Scale").Value : "";
            */

            // idk
            List<string> scales = new() { ".scale-100", ".scale-200", ".scale-300", ".scale-400" };

            var resApp = xml.Element(ns + "Package")
                                .Element(ns + "Applications")
                                .Elements(ns + "Application")
                                .FirstOrDefault(e => e.Attribute("Id") != null && e.Attribute("Id").Value == entrypoint);
            string logo = resApp.Element(uap + "VisualElements").Attribute("Square44x44Logo").Value;

            string ext = Path.GetExtension(logo);
            string dir = Path.GetDirectoryName(logo);

            foreach (string scale in scales)
            {
                string finalPath = Path.Combine(path, dir, Path.GetFileNameWithoutExtension(logo) + scale + ext);
                Debug.WriteLine(finalPath);
                if (File.Exists(finalPath))
                {
                    Debug.WriteLine("found:" + finalPath);
                    return finalPath;
                }
            }
            return null;
        }
        private static string[] GetPackages(string familyName)
        {
            uint count = 0;
            uint bufferLength = 0;

            // get size
            GetPackagesByPackageFamily(familyName, ref count, IntPtr.Zero, ref bufferLength, IntPtr.Zero);

            IntPtr packageFullNamesPtr = Marshal.AllocHGlobal((int)(count * IntPtr.Size));
            IntPtr bufferPtr = Marshal.AllocHGlobal((int)bufferLength * sizeof(char));

            try
            {
                GetPackagesByPackageFamily(familyName, ref count, packageFullNamesPtr, ref bufferLength, bufferPtr);
                string[] packageFullNames = new string[count];
                for (int i = 0; i < count; i++)
                {
                    IntPtr ptr = Marshal.ReadIntPtr(packageFullNamesPtr, i * IntPtr.Size);
                    packageFullNames[i] = Marshal.PtrToStringUni(ptr);
                }
                return packageFullNames;
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
                Marshal.FreeHGlobal(packageFullNamesPtr);
            }
        }
        // I'm so sorry for this entire thing it's so incredibly cursed
        public Tuple<string, string> GetPackageFromAppHandle(IntPtr hWnd)
        {
            Guid guidPropertyStore = new Guid("{886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99}");
            IPropertyStore propertyStore;
            int result = SHGetPropertyStoreForWindow(hWnd, ref guidPropertyStore, out propertyStore);
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

            if (string.IsNullOrWhiteSpace(aumid) || !aumid.Contains('!'))
            {
                return null;
            }

            string[] aumidParts = aumid.Split('!');
            string packageFamilyName = aumidParts[0];
            string entrypoint = aumidParts[1];

            // should return 1 package, idk
            string[] packages = GetPackages(packageFamilyName);
            string package = packages.Last();


            uint pathLength = 0;

            GetPackagePathByFullName(package, ref pathLength, IntPtr.Zero);
            IntPtr pathPtr = Marshal.AllocHGlobal((int)(pathLength * sizeof(char)));
            if (pathLength > 0)
            {
                result = GetPackagePathByFullName(package, ref pathLength, pathPtr);
                string packagePath = Marshal.PtrToStringUni(pathPtr);
                Debug.WriteLine(packagePath + ", aumid: " + aumid);
                Marshal.FreeHGlobal(pathPtr);

                return new Tuple<string, string>(packagePath, entrypoint);
            }
            return null;
        }
    }
}
