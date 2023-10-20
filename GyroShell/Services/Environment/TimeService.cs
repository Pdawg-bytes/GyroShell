using GyroShell.Library.Services.Environment;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Services.Environment
{
    internal class TimeService : ITimeService
    {
        private string _clockFormat = "t";
        public string ClockFormat
        {
            get => _clockFormat;
            set => _clockFormat = value;
        }

        public string ClockText
        {
            get => DateTime.Now.ToString(ClockFormat);
        }

        private string _dateText;
        public string DateText
        {
            get => DateTime.Now.ToString("M/d/yyyy");
        }
    }
}
