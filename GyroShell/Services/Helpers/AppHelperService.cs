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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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

            string iconPath = Uri.UnescapeDataString(xml.Element(ns + "Package")
                                          .Element(ns + "Properties")
                                          .Element(ns + "Logo").Value);

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
                                .FirstOrDefault(e => e.Attribute("Executable") != null && e.Attribute("Executable").Value == entrypoint);
            string logo = resApp.Element(uap + "VisualElements").Attribute("Square44x44Logo").Value;

            string ext = Path.GetExtension(iconPath);
            string dir = Path.GetDirectoryName(iconPath);

            foreach (string scale in scales)
            {
                string finalPath = Path.Combine(path, dir, Path.GetFileNameWithoutExtension(logo) + scale + ext);
                if (File.Exists(finalPath))
                {
                    //Debug.WriteLine(finalPath);
                    return finalPath;
                }
            }
            return null;
        }
        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr> ?? throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            list.Add(handle);
            return true;
        }

        // I'm so sorry for this entire thing it's so incredibly cursed
        public Tuple<string, string> GetPackageFromAppHandle(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out uint pid);
            var origTitle = GetWindowTitle(hWnd);
            IntPtr hProcess = OpenProcess(ProcessQueryLimitedInformation, false, pid);

            StringBuilder processName = new StringBuilder(256);
            GetProcessImageFileName(hProcess, processName, 256);
            var processname = processName.ToString().Split('\\').Reverse().ToArray()[0];



            uint realpid = 0;
            if (processname.ToLower() == "applicationframehost.exe")
            {
                var lis = GetChildWindows(hWnd);
                foreach (IntPtr wnd in lis)
                {
                    GetWindowThreadProcessId(wnd, out uint tmppid);
                    var s = GetWindowTitle(wnd);
                    if (!string.IsNullOrWhiteSpace(s) && origTitle.Contains(s))
                    {
                        //Debug.WriteLine("UWP realpid: " + tmppid.ToString());
                        realpid = tmppid;
                    }
                }
            }
            Marshal.FreeHGlobal(hProcess);

            if (realpid != 0)
            {
                hProcess = OpenProcess(ProcessQueryLimitedInformation, false, realpid);
                uint len = 0;
                GetPackageId(hProcess, ref len, IntPtr.Zero);
                if (len > 0)
                {
                    IntPtr infoBuffer = Marshal.AllocHGlobal((int)len);
                    GetPackageId(hProcess, ref len, infoBuffer);

                    var info = Marshal.PtrToStructure<PACKAGE_ID>(infoBuffer);

                    //Debug.WriteLine("UWP: " + Marshal.PtrToStringUni(info.name));

                    uint pathLength = 0;
                    GetPackagePath(ref info, 0, ref pathLength, IntPtr.Zero);
                    if (pathLength > 0)
                    {
                        IntPtr pathPtr = Marshal.AllocHGlobal((int)(pathLength * sizeof(char)));
                        GetPackagePath(ref info, 0, ref pathLength, pathPtr);
                        string packagePath = Marshal.PtrToStringUni(pathPtr);

                        StringBuilder uwpSb = new StringBuilder(256);
                        GetProcessImageFileName(hProcess, uwpSb, 256);
                        var uwpProcName = uwpSb.ToString().Split('\\').Reverse().ToArray()[0];

                        //Debug.WriteLine("UWP: " + packagePath + ", " + uwpProcName);

                        Marshal.FreeHGlobal(infoBuffer);
                        Marshal.FreeHGlobal(pathPtr);
                        return new Tuple<string, string>(packagePath, uwpProcName);
                    }
                }
            }
            return null;
        }
    }
}
