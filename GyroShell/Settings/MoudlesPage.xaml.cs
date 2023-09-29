// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using GyroShell.Helpers;
using GyroShell.Helpers.Modules;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Core;

using static GyroShell.Helpers.Modules.ModuleManager;

namespace GyroShell.Settings
{
    public sealed partial class MoudlesPage : Page
    {
        string? modulesPath = App.localSettings.Values["modulesFolderPath"] as string;

        internal ObservableCollection<ModuleModel> ModuleCollection;

        public MoudlesPage()
        {
            this.InitializeComponent();

            if (modulesPath != null)
            {
                try
                {
                    InitializeModuleList(modulesPath);
                    ModuleCollection = new ObservableCollection<ModuleModel>(GetModules());
                }
                catch
                {
                    ModuleParseErrorInfo.IsOpen = true;
                }
            }
            else 
            {
                ModuleNotFoundInfo.IsOpen = true;
            }

            int? iconStyle = App.localSettings.Values["iconStyle"] as int?;
            if (iconStyle != null)
            {
                switch (iconStyle)
                {
                    case 0:
                    default:
                        ModuleIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        break;
                    case 1:
                        ModuleIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                        break;
                }
            }
        }

        #region Control Events
        private async void OpenFolderInfo_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, MainWindow.hWnd);

            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                App.localSettings.Values["modulesFolderPath"] = folder.Path;
                try
                {
                    InitializeModuleList(modulesPath);
                    ModuleCollection = new ObservableCollection<ModuleModel>(GetModules());
                }
                catch 
                {
                    ModuleParseErrorInfo.IsOpen = true; 
                }
                ModuleNotFoundInfo.IsOpen = false;
            }
            else
            {
                ModuleNotFoundInfo.IsOpen = false;
            }
        }

        private void IgnoreInfo_Click(object sender, RoutedEventArgs e)
        {
            ModuleNotFoundInfo.IsOpen = false;
        }
        #endregion
    }
}
