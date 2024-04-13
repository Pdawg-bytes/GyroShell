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

using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Library.Models.InternalData
{
    public class IconModel : INotifyPropertyChanged
    {
        public void CloseWindow()
        {
            IntPtr retval = IntPtr.Zero;
            SendMessageTimeout(Id, WM_SYSCOMMAND, SC_CLOSE, 0, 2, 200, ref retval);
        }

        private string iconName;
        public string IconName
        {
            get { return iconName; }
            set
            {
                if (iconName != value)
                {
                    iconName = value;
                    OnPropertyChanged();
                }
            }
        }

        private SoftwareBitmapSource appIcon;
        public SoftwareBitmapSource AppIcon
        {
            get { return appIcon; }
            set
            {
                if (appIcon != value)
                {
                    appIcon = value;
                    OnPropertyChanged();
                }
            }
        }

        private WindowState _state;
        public WindowState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public IntPtr Id { get; set; }


        public enum WindowState
        {
            Active,
            Inactive,
            Flashing,
            Hidden
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}