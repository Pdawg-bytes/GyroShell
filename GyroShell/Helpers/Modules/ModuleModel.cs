using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GyroShell.Helpers.Modules
{
    internal class ModuleModel : INotifyPropertyChanged
    {
        public string ModuleName { get; set; }

        public string ModuleVersion { get; set; }

        public Guid ModuleId { get; set; }

        private bool isLoaded;
        public bool IsLoaded
        {
            get { return isLoaded; }
            set
            {
                if (isLoaded != value)
                {
                    isLoaded = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
