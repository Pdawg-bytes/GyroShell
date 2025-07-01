#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Services.Environment;
using System;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Library.ViewModels
{
    public sealed class AboutSettingViewModel
    {
        private readonly ISettingsService _appSettings;
        private readonly IEnvironmentInfoService _envService;

        public AboutSettingViewModel(ISettingsService appSettings, IEnvironmentInfoService envService)
        {
            _appSettings = appSettings;
            _envService = envService;
        }

        public string SystemArchitecture
        {
            get => _envService.SystemArchitecture;
        }

        public Version AppVersion
        {
            get => _envService.AppVersion;
        }

        public string AppBuildDate
        {
            get => _envService.AppBuildDate.ToString("MMMM dd, yyyy");
        }

        public FontFamily IconFontFamily => _appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };
    }
}
