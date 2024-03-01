using GyroShell.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Interfaces
{
    public interface IPlugin
    {
        public IPluginInfo PluginInformation { get; }

        void Initialize(IServiceProvider localServiceProvider);

        void Shutdown();
    }
}
