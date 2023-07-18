using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.System;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Helpers
{
    internal class IconModel : INotifyPropertyChanged
    {
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

        public IntPtr Id { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}