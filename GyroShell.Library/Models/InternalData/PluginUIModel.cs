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

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GyroShell.Library.Models.InternalData
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