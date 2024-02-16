using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GyroShell.Library.Models.InternalData
{
    public class PluginUIModel : INotifyPropertyChanged
    {
        public string PluginName { get; set; }

        public string FullName { get; set; }

        public string Description { get; set; }

        public string PublisherName { get; set; }

        public string PluginVersion { get; set; }

        public Guid PluginId { get; set; }

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