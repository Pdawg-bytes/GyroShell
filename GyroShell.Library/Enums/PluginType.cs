using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Enums
{
    public enum PluginType
    {
        /// <summary>
        /// A plugin that does not host any UI, but instead provides services or data.
        /// </summary>
        Backend,

        /// <summary>
        /// A plugin that hosts a UI inside of the existing GyroShell environment, such as a custom taskbar.
        /// </summary>
        InternalUI,

        /// <summary>
        /// A plugin that hosts an independent UI, such as a desktop widget.
        /// </summary>
        ExternalUI
    }
}
