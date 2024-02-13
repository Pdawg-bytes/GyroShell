using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static GyroShell.Library.Models.InternalData.IconModel;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using Windows.System;
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