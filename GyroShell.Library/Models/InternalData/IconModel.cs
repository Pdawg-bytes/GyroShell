using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GyroShell.Library.Models.publicData
{
    public class IconModel : INotifyPropertyChanged
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