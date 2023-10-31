using Microsoft.UI.Dispatching;

namespace GyroShell.Library.Services.Environment
{
    /// <summary>
    /// Provides an abstraction of GyroShell's internal dispatcher.
    /// </summary>
    public interface IDispatcherService
    {
        DispatcherQueue DispatcherQueue { get; init; }
    }
}
