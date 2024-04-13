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

using CommunityToolkit.Mvvm.ComponentModel;
using GyroShell.Library.Services.Environment;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Library.ViewModels
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        private readonly ISettingsService m_appSettings;
        
        public SettingsWindowViewModel(ISettingsService appSettings)
        {
            m_appSettings = appSettings;

            m_appSettings.SettingUpdated += AppSettings_SettingUpdated;
        }

        private void AppSettings_SettingUpdated(object sender, string key)
        {
            switch (key)
            {
                case "iconStyle": OnPropertyChanged(nameof(IconFontFamily)); break;
            }
        }

        public FontFamily IconFontFamily => m_appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };
    }
}
