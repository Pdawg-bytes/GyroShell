using GyroShell.Library.Helpers.Composition;

namespace GyroShell.Library.Helpers.Window
{
    internal partial class FlyoutWindow : ShellWindow
    {
        internal FlyoutWindow()
            : base(null, 300, 300,
                  customTransparency: false, 
                  customBackdrop: new TransparentTintBackdrop(new Windows.UI.Composition.Compositor(), new Windows.UI.Color { A = 0, R = 0, G = 0, B = 0 }))
        { }

        internal void ToggleWindow(bool show)
        {
            if (show) base.AppWindow?.Show();
            else base.AppWindow?.Hide();
        }
    }
}