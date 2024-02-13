using Microsoft.UI.Dispatching;
using GyroShell.Library.Services.Environment;

namespace GyroShell.Services.Environment
{
    public class DispatcherService : IDispatcherService
    {
        public DispatcherService() 
        {
            DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        public DispatcherQueue DispatcherQueue { get; init; }
    }
}
