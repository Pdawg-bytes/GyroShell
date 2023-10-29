using Microsoft.UI.Dispatching;

namespace GyroShell.Library.Services.Environment
{
    public interface IDispatcherService
    {
        DispatcherQueue DispatcherQueue { get; init; }
    }
}
