using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace GyroShell.Controls
{
    public sealed partial class RestartDialog : ContentDialog
    {
        public RestartDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            AppRestartFailureReason result = await CoreApplication.RequestRestartAsync("-fastInit -level 1 -foo");
            if (result == AppRestartFailureReason.NotInForeground || result == AppRestartFailureReason.Other)
            {
                SendErrorToast("GyroShell was unable to restart", "Please manually restart GyroShell by right clicking the start button, then click Exit GyroShell.");
            }
        }

        private bool SendErrorToast(string title, string message)
        {
            var xmlPayload = new string($@"
            <toast>    
                <visual>    
                    <binding template=""ToastGeneric"">    
                        <text>{title}</text>
                        <text>{message}</text>    
                    </binding>
                </visual>  
            </toast>");

            var toast = new AppNotification(xmlPayload);
            AppNotificationManager.Default.Show(toast);
            return toast.Id != 0;
        }
    }
}
