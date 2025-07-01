#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using CommunityToolkit.Mvvm.ComponentModel;
using GyroShell.Library.Services.Environment;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Library.ViewModels
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        private readonly ISettingsService _appSettings;
        
        public SettingsWindowViewModel(ISettingsService appSettings)
        {
            _appSettings = appSettings;

            _appSettings.SettingUpdated += AppSettings_SettingUpdated;
        }

        private void AppSettings_SettingUpdated(object sender, string key)
        {
            switch (key)
            {
                case "iconStyle": OnPropertyChanged(nameof(IconFontFamily)); break;
            }
        }

        public FontFamily IconFontFamily => _appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };
    }
}
