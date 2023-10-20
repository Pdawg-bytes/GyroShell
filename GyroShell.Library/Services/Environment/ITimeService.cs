using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Environment
{
    public interface ITimeService
    {
        /// <summary>
        /// The string to format the clock data with.
        /// </summary>
        public string ClockFormat { get; set; }

        /// <summary>
        /// The clock text.
        /// </summary>
        public string ClockText { get; }

        /// <summary>
        /// The date text.
        /// </summary>
        public string DateText { get; }
    }
}
