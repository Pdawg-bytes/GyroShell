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
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using static GyroShell.Library.Models.InternalData.IconModel;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using GyroShell.Library.Models.InternalData;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GyroShell.Controls
{
    public partial class TaskButton : UserControl
    {
        private IconModel window;

        private static Dictionary<WindowState, string> _visualStateStringPairs;

        public TaskButton()
        {
            this.InitializeComponent();
            _visualStateStringPairs = new Dictionary<WindowState, string>()
            {
                { WindowState.Active, "Active" },
                { WindowState.Inactive, "Inactive" },
                { WindowState.Flashing, "Flashing" },
                { WindowState.Hidden, "Hidden" }
            };
        }

        private void TaskButton_ActualThemeChanged(FrameworkElement sender, object args)
        {
            WindowState originalState = window.State;
            VisualStateManager.GoToState(this, "Unused", false);
            VisualStateManager.GoToState(this, _visualStateStringPairs[originalState], false);
        }

        private void TaskButton_Loaded(object sender, RoutedEventArgs e)
        {
            window = DataContext as IconModel;
            window.PropertyChanged += Window_PropertyChanged;

            VisualStateManager.GoToState(this, "Inactive", true);
            this.ActualThemeChanged += TaskButton_ActualThemeChanged;
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
                            VisualStateManager.GoToState(this, "Hidden", true);
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
                //SwitchToThisWindow(window.Id, false);
            }
            else
            {
                //ShowWindow(window.Id, 9);
                SwitchToThisWindow(window.Id, true);
            }
        }

        private void CloseWindowFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            window.CloseWindow();
        }
    }
}