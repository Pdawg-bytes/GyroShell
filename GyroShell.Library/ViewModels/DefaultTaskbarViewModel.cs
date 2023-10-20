using CommunityToolkit.Mvvm.ComponentModel;
using GyroShell.Library.Services.Environment;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.ViewModels
{
    public sealed class DefaultTaskbarViewModel : ObservableObject
    {
        private readonly ITimeService m_timeService;
        public DefaultTaskbarViewModel(ITimeService timeService) 
        {
            m_timeService = timeService;

            DispatcherTimer dateTimeUpdate = new DispatcherTimer();

            dateTimeUpdate.Tick += DateTimeUpdate;
            dateTimeUpdate.Interval = new TimeSpan(400000);

            dateTimeUpdate.Start();
        }

        private void DateTimeUpdate(object sender, object e)
        {
            OnPropertyChanged(nameof(ClockText));
            OnPropertyChanged(nameof(DateText));
        }
        

        public string ClockText
        {
            get => m_timeService.ClockText;
        }

        public string DateText
        {
            get => m_timeService.DateText;
        }
    }
}