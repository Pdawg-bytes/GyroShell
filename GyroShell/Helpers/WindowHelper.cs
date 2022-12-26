using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using static GyroShell.MainWindow;

class WindowsSystemDispatcherQueueHelper
{
    [StructLayout(LayoutKind.Sequential)]
    struct DispatcherQueueOptions
    {
        internal int dwSize;
        internal int threadType;
        internal int apartmentType;
    }

    [DllImport("CoreMessaging.dll")]
    private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

    object m_dispatcherQueueController = null;
    public void EnsureWindowsSystemDispatcherQueueController()
    {
        if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
        {
            // one already exists, so we'll just use it.
            return;
        }

        if (m_dispatcherQueueController == null)
        {
            DispatcherQueueOptions options;
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType = 2;    // DQTYPE_THREAD_CURRENT
            options.apartmentType = 2; // DQTAT_COM_STA

            CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
        }
    }
}
internal static class SW
{
    public const int HIDE = 0;
    public const int SHOWNORMAL = 1;
    public const int NORMAL = 1;
    public const int SHOWMINIMIZED = 2;
    public const int SHOWMAXIMIZED = 3;
    public const int MAXIMIZE = 3;
    public const int SHOWNOACTIVATE = 4;
    public const int SHOW = 5;
    public const int MINIMIZE = 6;
    public const int SHOWMINNOACTIVE = 7;
    public const int SHOWNA = 8;
    public const int RESTORE = 9;
    public const int SHOWDEFAULT = 10;
}
static class WindowExtensions
{
    [DllImport("user32.dll")]
    public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

    public static void SetWindowPlacement(this Window window, IntPtr hWnd, ref WINDOWPLACEMENT placement)
    {
        SetWindowPlacement(hWnd, ref placement);
    }
}