using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace GyroShell.ViewModels
{
    public class CustomizationViewModel : ObservableObject
    {
        public CustomizationViewModel()
        {

        }

        // 24 Hour View property
        [ObservableProperty]
        private bool _isTFHour;
        public bool IsTFHour
        {
            get { return _isTFHour; }
            set { _isTFHour = value; }
        }
    }
}
