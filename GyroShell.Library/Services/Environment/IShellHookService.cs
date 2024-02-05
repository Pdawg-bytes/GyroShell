using GyroShell.Library.Events;
using GyroShell.Library.Models.InternalData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Environment
{
    public interface IShellHookService
    {
        public void Initialize();

        public IntPtr MainWindowHandle { get; set; }

        public ObservableCollection<IconModel> CurrentWindows { get; }
    }
}
