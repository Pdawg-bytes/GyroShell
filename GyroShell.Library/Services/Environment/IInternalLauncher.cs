using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Environment
{
    /// <summary>
    /// Defines a list of entry points into GyroShell's internal backend.
    /// </summary>
    public interface IInternalLauncher
    {
        /// <summary>
        /// Launches the shell settings.
        /// </summary>
        public void LaunchShellSettings();

        /// <summary>
        /// Starts a new instance of GyroShell.
        /// </summary>
        /// <remarks>Kills the current instace. The new instance replaces the current one.</remarks>
        public void LaunchNewShellInstance();

        /// <summary>
        /// Exits the current instance of GyroShell (App.Current).
        /// </summary>
        public void ExitGyroShell();

        /// <summary>
        /// An abstraction of GyroShell's public ProcessStartEx method.
        /// </summary>
        /// <param name="procName">The name of the process to launch.</param>
        /// <param name="createNoWindow">Create no window flag.</param>
        /// <param name="useShellEx">Launches the process with ShellExecute.</param>
        public void LaunchProcess(string procName, bool createNoWindow, bool useShellEx);
    }
}
