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

namespace GyroShell.Controls
{
    public partial class TaskButton : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty AppIconProperty =
            DependencyProperty.Register("AppIcon", typeof(SoftwareBitmapSource), typeof(TaskButton), null);

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TaskButton), null);

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(WindowState), typeof(TaskButton),
            new PropertyMetadata(WindowState.Inactive, OnStateChanged));

        public static readonly DependencyProperty HandleProperty =
            DependencyProperty.Register("Handle", typeof(IntPtr), typeof(TaskButton), null);


        public TaskButton()
        {
            this.InitializeComponent();
        }


        private void BackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            if (State != WindowState.Active)
            {
                ShowWindow(Handle, 1);
            }
            else
            {
                ShowWindow(Handle, 0);
            }
        }


        public SoftwareBitmapSource AppIcon
        {
            get => (SoftwareBitmapSource)GetValue(AppIconProperty);
            set => SetValue(AppIconProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public IntPtr Handle
        {
            get { return (IntPtr)GetValue(HandleProperty); }
            set => SetValue(HandleProperty, value);
        }

        public WindowState State
        {
            get => (WindowState)GetValue(StateProperty);
            set
            {
                switch (value) 
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
            }
        }
        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskButton button = (TaskButton)d;
            if (e.NewValue != e.OldValue)
            {
                button.State = (WindowState)e.NewValue;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
