using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Environment
{
    public interface ITimeService
    {
        /// <summary>
        /// The formatter string for the clock.
        /// </summary>
        public string ClockFormat { get; set; }

        /// <summary>
        /// The formatter string for the date.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// The event that is raised to update frontend properties.
        /// </summary>
        public event EventHandler UpdateClockBinding;
    }
}
