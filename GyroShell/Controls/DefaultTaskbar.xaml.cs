using CommunityToolkit.WinUI.Connectivity;
using GyroShell.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.ApplicationModel.Core;
using Windows.Devices.Power;
using Windows.UI;
using System.Diagnostics;
using Windows.UI.Core;
using static GyroShell.Helpers.Win32Interop;
using Windows.System;
using Windows.UI.Notifications.Management;
using Windows.Foundation.Metadata;
using System.Collections.Generic;
using Windows.UI.Notifications;
using GyroShell.Settings;

namespace GyroShell.Controls
{
    public sealed partial class DefaultTaskbar : Page
    {
        public static int SettingInstances = 0;
        bool reportRequested = false;
        public static string timeType = "t";

        public DefaultTaskbar()
        {
            this.InitializeComponent();
            LoadSettings();
            TimeAndDate();
            DetectBatteryPresence();
            InternetUpdate();
            InitNotifcation();
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
            BarBorder.Background = new SolidColorBrush(Color.FromArgb(255,66,63,74));
            RightClockSeperator.Background = new SolidColorBrush(Color.FromArgb(255,120,120,120));
            LeftControlsSeperator.Background = new SolidColorBrush(Color.FromArgb(255, 120, 120, 120));
        }

        #region Clock
        private void TimeAndDate()
        {
            DispatcherTimer dateTimeUpdate = new DispatcherTimer();
            dateTimeUpdate.Tick += DTUpdateMethod;
            dateTimeUpdate.Interval = new TimeSpan(100000);
            dateTimeUpdate.Start();
        }
        private void DTUpdateMethod(object sender, object e)
        {
            TimeText.Text = DateTime.Now.ToString(timeType);
            DateText.Text = DateTime.Now.ToString("M");
        }
        #endregion

        #region Internet
        private void InternetUpdate()
        {
            DispatcherTimer internetUpdate = new DispatcherTimer();
            internetUpdate.Tick += ITUpdateMethod;
            internetUpdate.Interval = new TimeSpan(10000000);
            internetUpdate.Start();
        }
        private string[] wifiIcons = { "\uE871", "\uE872", "\uE873", "\uE874", "\uE701" };
        private string[] dataIcons = { "\uEC37", "\uEC38", "\uEC39", "\uEC3A", "\uEC3B" };
        private void ITUpdateMethod(object sender, object e)
        {
            if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                switch (NetworkHelper.Instance.ConnectionInformation.ConnectionType)
                {
                    case ConnectionType.Ethernet:
                        WifiStatus.Text = "\uE839";
                        break;
                    case ConnectionType.WiFi:
                        int WifiSignalBars = NetworkHelper.Instance.ConnectionInformation.SignalStrength.GetValueOrDefault(0);
                        WifiStatus.Text = wifiIcons[WifiSignalBars];
                        break;
                    case ConnectionType.Data:
                        int DataSignalBars = NetworkHelper.Instance.ConnectionInformation.SignalStrength.GetValueOrDefault(0);
                        WifiStatus.Text = dataIcons[DataSignalBars];
                        break;
                    case ConnectionType.Unknown:
                    default:
                        WifiStatus.Text = "\uE774";
                        break;
                }
            }
            else
            {
                WifiStatus.Text = "\uEB55";
            }
        }
        #endregion

        #region Battery
        private void DetectBatteryPresence()
        {
            var aggDetectBattery = Battery.AggregateBattery;
            var report = aggDetectBattery.GetReport();
            string ReportResult = report.Status.ToString();
            if (ReportResult == "NotPresent")
            {
                BattStatus.Visibility = Visibility.Collapsed;
            }
            else
            {
                BattStatus.Visibility = Visibility.Visible;
                reportRequested = true;
                AggregateBattery();
            }
        }

        string[] batteryIconsCharge = { "\uEBAE", "\uEBAC", "\uEBAD", "\uEBAE", "\uEBAF", "\uEBB0", "\uEBB1", "\uEBB2", "\uEBB3", "\uEBB4", "\uEBB5" };
        string[] batteryIcons = { "\uEBA0", "\uEBA1", "\uEBA2", "\uEBA3", "\uEBA4", "\uEBA5", "\uEBA6", "\uEBA7", "\uEBA8", "\uEBA9", "\uEBAA" };
        private void AggregateBattery()
        {
            var aggBattery = Battery.AggregateBattery;
            var report = aggBattery.GetReport();
            string charging = report.Status.ToString();
            double fullCharge = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
            double currentCharge = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
            double battLevel = Math.Ceiling((currentCharge / fullCharge) * 100);
            if (charging == "Charging" || charging == "Idle")
            {
                int indexCharge = (int)Math.Floor(battLevel / 10);
                BattStatus.Text = batteryIconsCharge[indexCharge];
            }
            else
            {
                int indexDischarge = (int)Math.Floor(battLevel / 10);
                BattStatus.Text = batteryIcons[indexDischarge];
            }
        }

        private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            if (reportRequested)
            {
                DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                {
                    AggregateBattery();
                });
            }
        }
        #endregion

        #region Sound

        #endregion

        #region Bar Events
        private async void SystemControls_Click(object sender, RoutedEventArgs e)
        {
            if (SystemControls.IsChecked == true)
            {
                if (OSVersion.IsWin11())
                {
                    await TaskbarManager.ToggleSysControl();
                }
                else
                {
                    await TaskbarManager.ToggleActionCenter();
                }
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartButton.IsChecked == true)
            {
                await TaskbarManager.ToggleStart();
            }
        }

        private async void ActionCenter_Click(object sender, RoutedEventArgs e)
        {
            if (ActionCenter.IsChecked == true)
            {
                await TaskbarManager.ToggleActionCenter();
            }
        }

        private void StartButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            StartFlyout.ShowAt(StartButton);
        }

        private async void StartFlyout_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem selectedItem)
            {
                string shellOption = selectedItem.Tag.ToString();
                switch (shellOption)
                {
                    case "ShellSettings":
                        SettingInstances++;
                        if (SettingInstances <= 1)
                        {
                            Settings.SettingsWindow settingsWindow = new Settings.SettingsWindow();
                            settingsWindow.Activate();
                        }
                        break;
                    case "ExitGyroShell":
                        App.Current.Exit();
                        break;
                    case "TaskMgr":
                        Process.Start(ProcessStart.ProcessStartEx("taskmgr.exe", false, true));
                        break;
                    case "Settings":
                        await Launcher.LaunchUriAsync(new Uri("ms-settings:"));
                        break;
                    case "FileExp":
                        Process.Start("explorer.exe");
                        break;
                    case "Run":
                        Process.Start("explorer.exe", "shell:::{2559a1f3-21d7-11d4-bdaf-00c04f60b9f0}");
                        break;
                }
            }
            else
            {
                throw new Exception("MenuFlyout sender variable error");
            }
        }

        private async void SysTray_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException("Systray not ready yet.");
            //await TaskbarManager.ShowSysTray(); /* Does nothing, no action lol*/
        }
        #endregion

        #region Notifications
        UserNotificationListener notifListener = UserNotificationListener.Current;
        private void InitNotifcation()
        {
            notifListener.NotificationChanged += Listener_NotificationChanged;
        }
        private async void Listener_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                UserNotificationListenerAccessStatus accessStatus = await notifListener.RequestAccessAsync();
                switch (accessStatus)
                {
                    case UserNotificationListenerAccessStatus.Allowed:
                        Customization.NotifError = false;
                        IReadOnlyList<UserNotification> notifsToast = await notifListener.GetNotificationsAsync(NotificationKinds.Toast);
                        IReadOnlyList<UserNotification> notifsOther = await notifListener.GetNotificationsAsync(NotificationKinds.Unknown);
                        if (notifsToast.Count > 0 || notifsOther.Count > 0)
                        {
                            DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                            {
                                NotifCircle.Visibility = Visibility.Visible;
                            });
                        }
                        else
                        {
                            DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                            {
                                NotifCircle.Visibility = Visibility.Collapsed;
                            });
                        }
                        break;
                    case UserNotificationListenerAccessStatus.Denied:
                        NotifCircle.Visibility = Visibility.Collapsed;
                        Customization.NotifError = true;
                        break;
                    case UserNotificationListenerAccessStatus.Unspecified:
                        Customization.NotifError = true;
                        break;
                }
            }
        }
        #endregion

        #region Settings
        private void LoadSettings()
        {
            // Icons
            int? iconStyle = App.localSettings.Values["iconStyle"] as int?;
            switch (iconStyle)
            {
                case 0:
                default:
                    Thickness wifiMarginM = WifiStatus.Margin;
                    wifiMarginM.Left = 0;
                    wifiMarginM.Top = -4;
                    wifiMarginM.Right = 7;
                    wifiMarginM.Bottom = 0;
                    WifiStatus.Margin = wifiMarginM;
                    WifiStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    Thickness sndMarginM = SndStatus.Margin;
                    sndMarginM.Left = 5;
                    sndMarginM.Top = 0;
                    sndMarginM.Right = 0;
                    sndMarginM.Bottom = 0;
                    SndStatus.Margin = sndMarginM;
                    SndStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    Thickness battMarginM = BattStatus.Margin;
                    battMarginM.Left = 0;
                    battMarginM.Top = 2;
                    battMarginM.Right = 12;
                    battMarginM.Bottom = 0;
                    BattStatus.Margin = battMarginM;
                    BattStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    NotifText.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    break;
                case 1:
                    if(OSVersion.IsWin11())
                    {
                        Thickness wifiMarginF = WifiStatus.Margin;
                        wifiMarginF.Left = 0;
                        wifiMarginF.Top = 2;
                        wifiMarginF.Right = 7;
                        wifiMarginF.Bottom = 0;
                        WifiStatus.Margin = wifiMarginF;
                        WifiStatus.FontFamily = new FontFamily("Segoe Fluent Icons");
                        Thickness sndMarginF = SndStatus.Margin;
                        sndMarginF.Left = 5;
                        sndMarginF.Top = 0;
                        sndMarginF.Right = 0;
                        sndMarginF.Bottom = 0;
                        SndStatus.Margin = sndMarginF;
                        SndStatus.FontFamily = new FontFamily("Segoe Fluent Icons");
                        Thickness battMarginF = BattStatus.Margin;
                        battMarginF.Left = 0;
                        battMarginF.Top = 3;
                        battMarginF.Right = 14;
                        battMarginF.Bottom = 0;
                        BattStatus.Margin = battMarginF;
                        BattStatus.FontFamily = new FontFamily("Segoe Fluent Icons");
                        NotifText.FontFamily = new FontFamily("Segoe Fluent Icons");
                    }
                    else
                    {
                        Thickness wifiMarginM1 = WifiStatus.Margin;
                        wifiMarginM1.Left = 0;
                        wifiMarginM1.Top = -4;
                        wifiMarginM1.Right = 7;
                        wifiMarginM1.Bottom = 0;
                        WifiStatus.Margin = wifiMarginM1;
                        WifiStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        Thickness sndMarginM1 = SndStatus.Margin;
                        sndMarginM1.Left = 5;
                        sndMarginM1.Top = 0;
                        sndMarginM1.Right = 0;
                        sndMarginM1.Bottom = 0;
                        SndStatus.Margin = sndMarginM1;
                        SndStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        Thickness battMarginM1 = BattStatus.Margin;
                        battMarginM1.Left = 0;
                        battMarginM1.Top = 2;
                        battMarginM1.Right = 12;
                        battMarginM1.Bottom = 0;
                        BattStatus.Margin = battMarginM1;
                        BattStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        NotifText.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    }
                    break;
            }

            // Button sizes
            if (!OSVersion.IsWin11())
            {
                // Start
                StartButton.CornerRadius = new CornerRadius(0);
                Thickness startMargin = StartButton.Margin;
                startMargin.Left = 0;
                StartButton.Margin = startMargin;

                // Controls & action center
                SystemControls.CornerRadius = new CornerRadius(0);
                ActionCenter.CornerRadius = new CornerRadius(0);
                Thickness actionMargin = ActionCenter.Margin;
                actionMargin.Right = 0;
                ActionCenter.Margin = actionMargin;

                // Systray
                SysTray.CornerRadius = new CornerRadius(0);
                Thickness trayMargin = SysTray.Margin;
                trayMargin.Left = 4;
                trayMargin.Top = 0;
                trayMargin.Right = 4;
                trayMargin.Bottom = 0;
                SysTray.Margin = trayMargin;
            }

            // Clock
            bool? secondsEnabled = App.localSettings.Values["isSeconds"] as bool?;
            bool? is24HREnabled = App.localSettings.Values["is24HR"] as bool?;
            if (secondsEnabled == true && is24HREnabled == true)
            {
                timeType = "H:mm:ss";
            }
            else if (secondsEnabled == true && is24HREnabled == false) 
            {
                timeType = "T";
            }
            else if (secondsEnabled == false && is24HREnabled == true)
            {
                timeType = "H:mm";
            }
            else if (secondsEnabled == false && is24HREnabled == false)
            {
                timeType = "t";
            }
        }
        #endregion
    }
}
