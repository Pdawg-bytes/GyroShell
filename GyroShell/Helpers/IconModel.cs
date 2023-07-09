using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GyroShell.Helpers
{
    internal class IconModel : INotifyPropertyChanged
    {
        private string iconName;
        internal string IconName
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

        /*private WriteableBitmap appIcon;
        internal WriteableBitmap AppIcon
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
        }*/

        internal IntPtr Id { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}