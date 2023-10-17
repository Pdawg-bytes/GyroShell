// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using GyroShell.Helpers.Modules;
using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;

using static GyroShell.Helpers.Modules.ModuleManager;

namespace GyroShell.Settings
{
    public sealed partial class MoudlesPage : Page
    {
        internal ObservableCollection<ModuleModel> ModuleCollection;

        private ISettingsService m_appSettings;
        private string m_modulesPath;

        public MoudlesPage()
        {
            this.InitializeComponent();

            m_appSettings = App.ServiceProvider.GetRequiredService<ISettingsService>();
            m_modulesPath = m_appSettings.ModulesFolderPath;

            if (m_modulesPath != null || m_modulesPath == string.Empty)
            {
                try
                {
                    InitializeModuleList(m_modulesPath);
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

            switch (m_appSettings.IconStyle)
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
                m_appSettings.ModulesFolderPath = folder.Path;
                try
                {
                    InitializeModuleList(m_modulesPath);
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
