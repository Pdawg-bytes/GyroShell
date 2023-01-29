using CommunityToolkit.WinUI.Connectivity;
using GyroShell.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Devices.Power;
using Windows.UI.Core;
using WindowsUdk.UI.Shell;

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
            TimeAndDate();
            LoadSettings();
            DetectBatteryPresence();
            InternetUpdate();
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
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
        private void ITUpdateMethod(object sender, object e)
        {
            if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                switch (NetworkHelper.Instance.ConnectionInformation.ConnectionType)
                {
                    case ConnectionType.Ethernet:
                        Thickness EthMargin = WifiStatus.Margin;
                        EthMargin.Top = 1;
                        WifiStatus.Text = "\uE839";
                        break;
                    case ConnectionType.WiFi:
                        Thickness WifiMargin = WifiStatus.Margin;
                        WifiMargin.Top = -2;
                        int WifiSignalBars = NetworkHelper.Instance.ConnectionInformation.SignalStrength.GetValueOrDefault(0);
                        switch (WifiSignalBars)
                        {
                            case 0:
                            default:
                                WifiStatus.Text = "\uE871";
                                break;
                            case 1:
                                WifiStatus.Text = "\uE872";
                                break;
                            case 2:
                                WifiStatus.Text = "\uE873";
                                break;
                            case 3:
                                WifiStatus.Text = "\uE874";
                                break;
                            case 4:
                                WifiStatus.Text = "\uE701";
                                break;
                        }
                        break;
                    case ConnectionType.Data:
                        Thickness DataMargin = WifiStatus.Margin;
                        DataMargin.Top = -2;
                        int DataSignalBars = NetworkHelper.Instance.ConnectionInformation.SignalStrength.GetValueOrDefault(0);
                        switch (DataSignalBars)
                        {
                            case 1:
                            default:
                                WifiStatus.Text = "\uEC37";
                                break;
                            case 2:
                                WifiStatus.Text = "\uEC38";
                                break;
                            case 3:
                                WifiStatus.Text = "\uEC39";
                                break;
                            case 4:
                                WifiStatus.Text = "\uEC3A";
                                break;
                            case 5:
                                WifiStatus.Text = "\uEC3B";
                                break;
                        }
                        break;
                    case ConnectionType.Unknown:
                    default:
                        Thickness UnknownMargin = WifiStatus.Margin;
                        UnknownMargin.Top = 0;
                        WifiStatus.Text = "\uE774";
                        break;
                }
            }
            else
            {
                WifiStatus.Text = "\uF140";
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
                ShellViewCoordinator controlsC = new ShellViewCoordinator(ShellView.ControlCenter);
                await controlsC.TryShowAsync(new ShowShellViewOptions());
            }
            else
            {

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
                ShellViewCoordinator actionC = new ShellViewCoordinator(ShellView.ActionCenter);
                await actionC.TryShowAsync(new ShowShellViewOptions());
            }
            else
            {

            }
        }

        private void StartButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            StartFlyout.ShowAt(StartButton);
        }

        private void StartFlyout_Click(object sender, RoutedEventArgs e)
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
                }
            }
            else
            {
                throw new Exception("MenuFlyout sender variable error");
            }
        }

        private void ExitGyroShell_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }
        #endregion

        public void UpdateIconService(bool Icon10Use)
        {
            if (Icon10Use)
            {
                BattStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                WifiStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                SndStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
            }
            else
            {
                BattStatus.FontFamily = new FontFamily("Segoe Fluent Icons");
                WifiStatus.FontFamily = new FontFamily("Segoe Fluent Icons");
                SndStatus.FontFamily = new FontFamily("Segoe Fluent Icons");
            }
        }

        private void LoadSettings()
        {
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

            int? iconStyle = App.localSettings.Values["iconStyle"] as int?;
            switch (iconStyle)
            {
                case 0:
                default:
                    break;
                case 1:
                    break;
            }
        }
    }
}
