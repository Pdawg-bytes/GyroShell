using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Events
{
    public class SystemTaskbarControlChangedEventArgs : EventArgs
    {
        public enum SystemControlChangedType
        {
            Start,
            ActionCenter,
            SystemControls
        }

        public SystemTaskbarControlChangedEventArgs(SystemControlChangedType type, bool value) 
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// The type of control that was changed on the taskbar.
        /// </summary>
        public SystemControlChangedType Type { get; }

        /// <summary>
        /// The value of the change.
        /// </summary>
        public bool Value { get; }
    }
}
