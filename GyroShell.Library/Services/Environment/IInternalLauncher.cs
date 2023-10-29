using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Environment
{
    public interface IInternalLauncher
    {
        /// <summary>
        /// Launches the shell settings.
        /// </summary>
        public void LaunchShellSettings();

        /// <summary>
        /// Starts a new instance of GyroShell.
        /// </summary>
        public void LaunchNewShellInstance();

        /// <summary>
        /// An abstraction of GyroShell's public ProcessStartEx method.
        /// </summary>
        /// <param name="procName">The name of the process to launch.</param>
        /// <param name="createNoWindow">Create no window flag.</param>
        /// <param name="useShellEx">Launches the process with ShellExecute.</param>
        public void LaunchProcess(string procName, bool createNoWindow, bool useShellEx);
    }
}
