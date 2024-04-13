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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using static GyroShell.Library.Models.InternalData.IconModel;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using GyroShell.Library.Models.InternalData;

namespace GyroShell.Controls
{
    public partial class TaskButton : UserControl
    {
        private IconModel window;

        public TaskButton()
        {
            this.InitializeComponent();
        }

        private void TaskButton_Loaded(object sender, RoutedEventArgs e)
        {
            window = DataContext as IconModel;
            window.PropertyChanged += Window_PropertyChanged;

            VisualStateManager.GoToState(this, "Inactive", true);
        }

        private void Window_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "State":
                    switch (window.State)
                    {
                        case WindowState.Inactive:
                        default:
                            VisualStateManager.GoToState(this, "Inactive", true);
                            break;
                        case WindowState.Active:
                            VisualStateManager.GoToState(this, "Active", true);
                            break;
                        case WindowState.Flashing:
                            VisualStateManager.GoToState(this, "Flashing", true);
                            break;
                        case WindowState.Hidden:
                            break;
                    }
                    break;
            }
        }

        private void BackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            if (window.State == WindowState.Active)
            {
                ShowWindow(window.Id, 6);
            }
            else
            {
                ShowWindow(window.Id, 9);
            }
        }

        private void CloseWindowFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            window.CloseWindow();
        }
    }
}