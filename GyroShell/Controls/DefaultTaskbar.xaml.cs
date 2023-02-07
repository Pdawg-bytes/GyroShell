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
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
            BarBorder.Background = new SolidColorBrush(Color.FromArgb(255,66,63,74));
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

        private void SysTray_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement
            throw new NotImplementedException("Systray not ready yet.");
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
                    wifiMarginM.Top = -2;
                    wifiMarginM.Right = 5;
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
                    battMarginM.Right = 10;
                    battMarginM.Bottom = 0;
                    BattStatus.Margin = battMarginM;
                    BattStatus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    break;
                case 1:
                    Thickness wifiMarginF = WifiStatus.Margin;
                    wifiMarginF.Left = 0;
                    wifiMarginF.Top = 2;
                    wifiMarginF.Right = 5;
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
                    battMarginF.Right = 12;
                    battMarginF.Bottom = 0;
                    BattStatus.Margin = battMarginF;
                    BattStatus.FontFamily = new FontFamily("Segoe Fluent Icons");
                    break;
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
