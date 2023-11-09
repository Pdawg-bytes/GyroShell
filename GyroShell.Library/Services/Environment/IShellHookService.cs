using GyroShell.Library.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Environment
{
    public interface IShellHookService
    {
        /// <summary>
        /// Sets the handle of the MainWindow for use internally.
        /// </summary>
        public IntPtr MainWindowHandle { get; set; }

        /// <summary>
        /// The event that is fired when a shellhook event occurs.
        /// </summary>
        /// <remarks>
        /// Provides data about the event, however you must handle the data in your ViewModel.
        /// </remarks>
        public event EventHandler<ShellHookEventArgs> ShellHookEvent;
    }
}
