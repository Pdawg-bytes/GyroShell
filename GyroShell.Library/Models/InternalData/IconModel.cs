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