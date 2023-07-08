using CommunityToolkit.WinUI.Connectivity;
using GyroShell.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Devices.Power;
using Windows.UI;
using System.Diagnostics;
using Windows.UI.Core;
using static GyroShell.Helpers.Win32.Win32Interop;
using static GyroShell.Helpers.Win32.GetWindowName;
using static GyroShell.Helpers.Win32.WindowChecks;
using Windows.System;
using Windows.UI.Notifications.Management;
using Windows.Foundation.Metadata;
using System.Collections.Generic;
using Windows.UI.Notifications;
using GyroShell.Settings;
using System.Collections.ObjectModel;
using Windows.Networking.Connectivity;
using CoreAudio;
using System.Threading;
using System.Linq;

namespace GyroShell.Controls
{
    public sealed partial class DefaultTaskbar : Page
    {
        public static int SettingInstances = 0;
        private int currentVolume;

        bool reportRequested = false;

        public static string timeType = "t";

        public ObservableCollection<IconModel> TbIconCollection;
        internal static List<IntPtr> indexedWindows = new List<IntPtr>();

        private readonly WinEventDelegate callback;

        public DefaultTaskbar()
        {
            this.InitializeComponent();

            TbIconCollection = new ObservableCollection<IconModel>();

            LoadSettings();
            TimeAndDate();
            DetectBatteryPresence();
            InitNotifcation();
            UpdateNetworkStatus();
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            AudioBackend.audioDevice.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
            AudioCheck();
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
            BarBorder.Background = new SolidColorBrush(Color.FromArgb(255, 66, 63, 74));

            callback = WinEventCallback;
            GetCurrentWindows();
            RegisterWinEventHook();
            TaskbarManager.SendWinlogonShowShell();
        }

        #region Clock
        private void TimeAndDate()
        {
            DispatcherTimer dateTimeUpdate = new DispatcherTimer();
            dateTimeUpdate.Tick += DTUpdateMethod;
            dateTimeUpdate.Interval = new TimeSpan(400000);
            dateTimeUpdate.Start();
        }
        private void DTUpdateMethod(object sender, object e)
        {
            TimeText.Text = DateTime.Now.ToString(timeType);
            DateText.Text = DateTime.Now.ToString("M/d/yyyy");
        }
        #endregion

        #region Internet
        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
            {
                UpdateNetworkStatus();
            });
        }
        private string[] wifiIcons = { "\uE871", "\uE872", "\uE873", "\uE874", "\uE701" };
        private string[] dataIcons = { "\uEC37", "\uEC38", "\uEC39", "\uEC3A", "\uEC3B" };
        private void UpdateNetworkStatus()
        {
            if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                switch (NetworkHelper.Instance.ConnectionInformation.ConnectionType)
                {
                    case ConnectionType.Ethernet:
                        WifiStatus.Text = "\uE839";
                        WifiStatus.Margin = new Thickness(0, 2, 7, 0);
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
        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            AudioCheck();
        }
        private void AudioCheck()
        {
            currentVolume = (int)Math.Ceiling(AudioBackend.audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100) - 1;
            if (currentVolume == 0 || currentVolume == -1)
            {
                DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                {
                    SndStatus.Text = "\uE992";
                });
            }
            else if (currentVolume <= 33)
            {
                DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                {
                    SndStatus.Text = "\uE993";
                });
            }
            else if (currentVolume <= 66)
            {
                DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                {
                    SndStatus.Text = "\uE994";
                });
            }
            else if (currentVolume <= 100)
            {
                DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                {
                    SndStatus.Text = "\uE995";
                });
            }
            else
            {
                DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                {
                    SndStatus.Text = "\uEA85";
                });
            }
        }
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
                            SettingsWindow settingsWindow = new SettingsWindow();
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
            FontFamily SegoeMDL2 = new FontFamily("Segoe MDL2 Assets");
            FontFamily SegoeFluent = new FontFamily("Segoe Fluent Icons");
            // Icons
            int? iconStyle = App.localSettings.Values["iconStyle"] as int?;
            switch (iconStyle)
            {
                case 0:
                default:
                    WifiStatus.Margin = new Thickness(0, -2, 7, 0);
                    WifiStatus.FontFamily = SegoeMDL2;
                    SndStatus.Margin = new Thickness(6, 1, 0, 0);
                    SndStatus.FontFamily = SegoeMDL2;
                    BattStatus.Margin = new Thickness(0, 2, 12, 0);
                    BattStatus.FontFamily = SegoeMDL2;
                    NotifText.FontFamily = SegoeMDL2;
                    //SearchIcon.FontFamily = SegoeMDL2;
                    TaskViewIcon.FontFamily = SegoeMDL2;
                    break;
                case 1:
                    if (OSVersion.IsWin11())
                    {
                        WifiStatus.Margin = new Thickness(0, 2, 7, 0);
                        WifiStatus.FontFamily = SegoeFluent;
                        SndStatus.Margin = new Thickness(5, 0, 0, 0);
                        SndStatus.FontFamily = SegoeFluent;
                        BattStatus.Margin = new Thickness(0, 3, 14, 0);
                        BattStatus.FontFamily = SegoeFluent;
                        NotifText.FontFamily = SegoeFluent;
                        //SearchIcon.FontFamily = SegoeFluent;
                        TaskViewIcon.FontFamily = SegoeFluent;
                    }
                    else
                    {
                        WifiStatus.Margin = new Thickness(0, -2, 7, 0);
                        WifiStatus.FontFamily = SegoeMDL2;
                        SndStatus.Margin = new Thickness(6, 1, 0, 0);
                        SndStatus.FontFamily = SegoeMDL2;
                        BattStatus.Margin = new Thickness(0, 2, 12, 0);
                        BattStatus.FontFamily = SegoeMDL2;
                        NotifText.FontFamily = SegoeMDL2;
                        //SearchIcon.FontFamily = SegoeMDL2;
                        TaskViewIcon.FontFamily = SegoeMDL2;
                    }
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

        #region Callbacks
        private void GetCurrentWindows()
        {
            EnumWindows(EnumWindowsCallbackMethod, IntPtr.Zero);
        }

        private IntPtr foregroundHook;
        private IntPtr cloakedHook;
        private IntPtr nameChangeHook;
        private IntPtr cdWindowHook;
        private void RegisterWinEventHook()
        {
            foregroundHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            cloakedHook = SetWinEventHook(EVENT_OBJECT_CLOAKED, EVENT_OBJECT_UNCLOAKED, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            nameChangeHook = SetWinEventHook(EVENT_OBJECT_NAMECHANGED, EVENT_OBJECT_NAMECHANGED, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            cdWindowHook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_DESTROY, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
        }

        // WinEvent Callbacks
        private void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            string windowName = GetWindowTitle(hwnd);
            if (!indexedWindows.Contains(hwnd)) 
            {
                if(isUserWindow(hwnd))
                {
                    indexedWindows.Add(hwnd);
                    Thread.Sleep(10);
                }
            }
            if (indexedWindows.Contains(hwnd))
            {
                switch (eventType)
                {
                    case EVENT_OBJECT_CREATE:
                        Thread.Sleep(1);
                        if (!indexedWindows.Contains(hwnd))
                        {
                            indexedWindows.Add(hwnd);
                            TbIconCollection.Add(new IconModel { IconName = windowName, Id = hwnd });
                        }
                        break;
                    case EVENT_OBJECT_NAMECHANGED:
                        try
                        {
                            IconModel icon = TbIconCollection.First(param => param.Id == hwnd);
                            icon.IconName = windowName;
                        }
                        catch
                        {
                            if (!TbIconCollection.Any(item => item.Id == hwnd))
                            {
                                TbIconCollection.Add(new IconModel { IconName = windowName, Id = hwnd });
                            }
                            Debug.WriteLine("[-] WinEventHook: Value not found in rename list.");
                        }
                        Debug.WriteLine("Window namechange: " + windowName + " | Handle: " + hwnd);
                        break;
                    case EVENT_SYSTEM_FOREGROUND:
                        IconModel targetItem = TbIconCollection.FirstOrDefault(item => item.Id == hwnd);
                        TbOpenGrid.SelectedItem = targetItem;

                        foreach (IconModel item in TbOpenGrid.Items)
                        {
                            GridViewItem container = TbOpenGrid.ContainerFromItem(item) as GridViewItem;
                            if (container != null)
                            {
                                VisualStateManager.GoToState(container, item == TbOpenGrid.SelectedItem ? "Pressed" : "Normal", true);
                            }
                        }
                        break;
                    case EVENT_OBJECT_CLOAKED:
                        indexedWindows.Remove(hwnd);
                        try
                        {
                            TbIconCollection.Remove(TbIconCollection.First(param => param.Id == hwnd));
                        }
                        catch
                        {
                            Debug.WriteLine("[-] WinEventHook EOC: Value not found in list.");
                        }
                        Debug.WriteLine("Window cloaked: " + windowName + " | Handle: " + hwnd);
                        break;
                    case EVENT_OBJECT_UNCLOAKED:
                        Debug.WriteLine("Window uncloaked: " + windowName + " | Handle: " + hwnd);
                        break;
                    case EVENT_OBJECT_DESTROY:
                        indexedWindows.Remove(hwnd);
                        try
                        {
                            TbIconCollection.Remove(TbIconCollection.First(param => param.Id == hwnd));
                        }
                        catch
                        {
                            Debug.WriteLine("[-] WinEventHook EOD: Value not found in list.");
                        }
                        Debug.WriteLine("Window destroy: " + windowName + " | Handle: " + hwnd);
                        break;
                }
            }
        }

        private bool EnumWindowsCallbackMethod(IntPtr hwnd, IntPtr lParam)
        {
            if (isUserWindow(hwnd)) { indexedWindows.Add(hwnd); TbIconCollection.Add(new IconModel { IconName = GetWindowTitle(hwnd), Id = hwnd }); }
            return true;
        }
        #endregion

        private void Icon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IconModel iconModel = ((FrameworkElement)sender).DataContext as IconModel;
            SetForegroundWindow(iconModel.Id);
            if (IsIconic(iconModel.Id))
            {
                ShowWindow(iconModel.Id, SW_RESTORE);
            }
        }

        private void Icon_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }
    }
}
