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
        private readonly ISettingsService m_appSettings;
        private readonly IEnvironmentInfoService m_envService;

        public AboutSettingViewModel(ISettingsService appSettings, IEnvironmentInfoService envService)
        {
            m_appSettings = appSettings;
            m_envService = envService;
        }

        public string SystemArchitecture
        {
            get => m_envService.SystemArchitecture;
        }

        public Version AppVersion
        {
            get => m_envService.AppVersion;
        }

        public string AppBuildDate
        {
            get => m_envService.AppBuildDate.ToString("MMMM dd, yyyy");
        }

        public FontFamily IconFontFamily => m_appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };
    }
}
