using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace GyroShell.Library.Controls
{
    public sealed partial class FlyoutControl : UserControl
    {
        FlyoutWindow containerWindow;
        public FlyoutControl()
        {
            this.InitializeComponent();
            containerWindow = new FlyoutWindow();
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("FlyoutContent", typeof(UIElement), typeof(FlyoutWindow), null);

        public UIElement FlyoutContent
        {
            get => (UIElement)GetValue(ContentProperty);
            set
            {
                SetValue(ContentProperty, value);
                containerWindow.Content = value;
                containerWindow.Activate();
            }
        }
    }
}