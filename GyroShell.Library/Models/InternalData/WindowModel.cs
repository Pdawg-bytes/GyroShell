#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Library.Models.InternalData
{
    public class WindowModel : INotifyPropertyChanged
    {
        private string iconName;
        public string WindowName
        {
            get => iconName;
            set
            {
                if (iconName != value)
                {
                    iconName = value;
                    OnPropertyChanged();
                }
            }
        }

        private ImageSource appIcon;
        public ImageSource AppIcon
        {
            get => appIcon;
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

        private uint _processId;
        public uint ProcessId
        {
            get
            {
                if (_processId == 0)
                    { GetWindowThreadProcessId(Id, out _processId); return _processId; }
                else
                    return _processId;
            }
        }

        public IntPtr CloseWindow()
        {
            IntPtr returnValue = IntPtr.Zero;
            SendMessageTimeout(Id, WM_SYSCOMMAND, SC_CLOSE, 0, 2, 200, ref returnValue);
            return returnValue;
        }

        public int WindowStyle
        {
            get => (int)GetWindowLongPtr(Id, GWL_STYLE);
        }

        public int ExtendedWindowStyle
        {
            get => (int)GetWindowLongPtr(Id, GWL_EXSTYLE);
        }


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