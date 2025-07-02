﻿#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GyroShell.Library.Models.Plugins
{
    public class PluginUIModel : INotifyPropertyChanged
    {
        public string PluginName { get; set; }

        public string FullName { get; set; }

        public string Description { get; set; }

        public string PublisherName { get; set; }

        public string PluginVersion { get; set; }

        public Guid PluginId { get; set; }

        private bool isLoaded;
        public bool IsLoaded
        {
            get => isLoaded;
            set
            {
                if (isLoaded != value)
                {
                    isLoaded = value; OnPropertyChanged();
                }
            }
        }

        private bool isLoadingAllowed;
        public bool IsLoadingAllowed
        {
            get => isLoadingAllowed;
            set
            {
                if (isLoadingAllowed != value)
                {
                    isLoadingAllowed = value; OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}