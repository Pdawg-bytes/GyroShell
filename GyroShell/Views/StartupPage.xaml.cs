using GyroShell.Library.Services.Environment;
using GyroShell.Library.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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

namespace GyroShell.Views
{
    public sealed partial class StartupPage : Page
    {
        public StartupPage()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<StartupScreenViewModel>();
        }

        public StartupScreenViewModel ViewModel => (StartupScreenViewModel)this.DataContext;
    }
}
