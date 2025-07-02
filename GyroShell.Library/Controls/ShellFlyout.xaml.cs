using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GyroShell.Library.Helpers.Window;


namespace GyroShell.Library.Controls
{
    public sealed partial class ShellFlyout : UserControl
    {
        private readonly FlyoutWindow containerWindow;

        public ShellFlyout()
        {
            this.InitializeComponent();
            containerWindow = new FlyoutWindow();
        }

        public static readonly DependencyProperty FlyoutContentProperty =
            DependencyProperty.Register("FlyoutContent", typeof(UIElement), typeof(FlyoutWindow), null);

        public static readonly DependencyProperty IsOpenProperty = 
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(FlyoutWindow), null);

        public UIElement FlyoutContent
        {
            get => (UIElement)GetValue(FlyoutContentProperty);
            set
            {
                SetValue(FlyoutContentProperty, value);
                containerWindow.Content = value;
                containerWindow.Activate();
            }
        }

        public bool IsOpen
        {
            get => containerWindow.Visible;
            set => containerWindow.ToggleWindow(value);
        }
    }
}