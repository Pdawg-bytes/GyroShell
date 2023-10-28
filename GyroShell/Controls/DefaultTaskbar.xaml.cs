using GyroShell.Helpers;
using GyroShell.Library.Models.Hardware;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using GyroShell.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

using static GyroShell.Library.Helpers.Win32.Win32Interop;
using static GyroShell.Library.Helpers.Win32.WindowChecks;
using BatteryReport = GyroShell.Library.Models.Hardware.BatteryReport;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.ViewModels;

namespace GyroShell.Controls
{
    public sealed partial class DefaultTaskbar : Page
    {
        public static string timeType = "t";

        private int currentVolume;
        private bool reportRequested = false;

        private IEnvironmentInfoService m_envService;
        private ISettingsService m_appSettings;

        private IAppHelperService m_appHelper;
        private IBitmapHelperService m_bmpHelper;

        private ITaskbarManagerService m_tbManager;

        private INetworkService m_netService;
        private IBatteryService m_powerService;
        private ISoundService m_soundService;

        private readonly WinEventDelegate callback;

        internal ObservableCollection<IconModel> TbIconCollection;
        internal static List<IntPtr> indexedWindows = new List<IntPtr>();

        public DefaultTaskbar()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<DefaultTaskbarViewModel>();

            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>();
            m_appSettings = App.ServiceProvider.GetRequiredService<ISettingsService>();

            m_appHelper = App.ServiceProvider.GetRequiredService<IAppHelperService>();
            m_bmpHelper = App.ServiceProvider.GetRequiredService<IBitmapHelperService>();

            m_tbManager = App.ServiceProvider.GetRequiredService<ITaskbarManagerService>();

            m_netService = App.ServiceProvider.GetRequiredService<INetworkService>();
            m_powerService = App.ServiceProvider.GetRequiredService<IBatteryService>();
            m_soundService = App.ServiceProvider.GetRequiredService<ISoundService>();

            TbIconCollection = new ObservableCollection<IconModel>();

            LoadSettings();
            TimeAndDate();
            DetectBatteryPresence();
            InitNotifcation();
            UpdateNetworkStatus();

            m_netService.InternetStatusChanged += NetworkService_InternetStatusChanged;
            m_soundService.OnVolumeChanged += SoundService_OnVolumeChanged;

            AudioCheck();

            m_powerService.BatteryStatusChanged += BatteryService_BatteryStatusChanged;

            BarBorder.Background = new SolidColorBrush(Color.FromArgb(255, 66, 63, 74));

            callback = WinEventCallback;
            GetCurrentWindows();
            RegisterWinEventHook();

            m_tbManager.NotifyWinlogonShowShell();
        }

        public DefaultTaskbarViewModel ViewModel => (DefaultTaskbarViewModel)this.DataContext;

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
        private void NetworkService_InternetStatusChanged(object sender, EventArgs e)
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
            if (m_netService.IsInternetAvailable)
            {
                switch (m_netService.InternetType)
                {
                    case InternetConnection.Wired:
                        WifiStatus.Text = "\uE839";
                        WifiStatus.Margin = new Thickness(0, 2, 7, 0);
                        break;
                    case InternetConnection.Wireless:
                        int WifiSignalBars = m_netService.SignalStrength;

                        WifiStatus.Text = wifiIcons[WifiSignalBars];
                        break;
                    case InternetConnection.Data:
                        int DataSignalBars = m_netService.SignalStrength;

                        WifiStatus.Text = dataIcons[DataSignalBars];
                        break;
                    case InternetConnection.Unknown:
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
            BatteryReport report = m_powerService.GetStatusReport();
            BatteryPowerStatus status = report.PowerStatus;

            if (status == BatteryPowerStatus.NotInstalled)
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
            BatteryReport report = m_powerService.GetStatusReport();
            double battLevel = report.ChargePercentage;
            BatteryPowerStatus status = report.PowerStatus;

            if (status == BatteryPowerStatus.Charging || status == BatteryPowerStatus.Idle)
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

        private void BatteryService_BatteryStatusChanged(object sender, EventArgs e)
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
        private void SoundService_OnVolumeChanged(object sender, EventArgs e)
        {
            AudioCheck();
        }
        private void AudioCheck()
        {
            currentVolume = m_soundService.Volume;

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

            if (m_soundService.IsMuted)
            {
                DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                {
                    SndStatus.Text = "\uE198";
                });
            }
        }
        #endregion

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
                    case "ExitGyroShell":
                        DestroyHooks();
                        App.Current.Exit();
                        break;
                    case "Desktop":
                        foreach (IntPtr handle in indexedWindows)
                        {
                            ShowWindow(handle, SW_MINIMIZE);
                        }
                        break;
                    case "SignOut":
                        ExitWindowsEx(EWX_LOGOFF, 0);
                        break;
                    case "Sleep":
                        SetSuspendState(false, false, false);
                        break;
                }
            }
        }

        private void MainShellGrid_Loaded(object sender, RoutedEventArgs e)
        {
            App.startupScreen.Close();
        }

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
                        CustomizationSettingView.NotifError = false;
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
                        CustomizationSettingView.NotifError = true;
                        break;
                    case UserNotificationListenerAccessStatus.Unspecified:
                        CustomizationSettingView.NotifError = true;
                        break;
                }
            }
        }
        #endregion

        #region Settings
        private void LoadSettings()
        {
            // Clock
            bool secondsEnabled = m_appSettings.EnableSeconds;
            bool is24HREnabled = m_appSettings.EnableMilitaryTime;
            timeType = secondsEnabled ? (is24HREnabled ? "H:mm:ss" : "T") : (is24HREnabled ? "H:mm" : "t");
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

        private IntPtr lastWindow;
        private void RegisterWinEventHook()
        {
            foregroundHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            cloakedHook = SetWinEventHook(EVENT_OBJECT_CLOAKED, EVENT_OBJECT_UNCLOAKED, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            nameChangeHook = SetWinEventHook(EVENT_OBJECT_NAMECHANGED, EVENT_OBJECT_NAMECHANGED, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            cdWindowHook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_DESTROY, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
        }
        private void DestroyHooks()
        {
            UnhookWinEvent(foregroundHook);
            UnhookWinEvent(cloakedHook);
            UnhookWinEvent(nameChangeHook);
            UnhookWinEvent(cdWindowHook);
        }
        // WinEvent Callback
        private void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            string windowName = m_appHelper.GetWindowTitle(hwnd);
            if (!indexedWindows.Contains(hwnd))
            {
                if (hwnd != IntPtr.Zero && isUserWindow(hwnd))
                {
                    indexedWindows.Add(hwnd);
                }
            }
            if (indexedWindows.Contains(hwnd))
            {
                switch (eventType)
                {
                    case EVENT_OBJECT_CREATE:
                        if (!TbIconCollection.Any(item => item.Id == hwnd))
                        {
                            indexedWindows.Add(hwnd);
                            SoftwareBitmapSource bmpSource = m_bmpHelper.GetXamlBitmapFromGdiBitmapAsync(m_appHelper.GetUwpOrWin32Icon(hwnd, 32)).Result;
                            TbIconCollection.Add(new IconModel { IconName = windowName, Id = hwnd, AppIcon = bmpSource });
                            if(GetForegroundWindow() == hwnd)
                            {
                                IconModel targetItemC = TbIconCollection.FirstOrDefault(item => item.Id == hwnd);
                                TbOpenGrid.SelectedItem = targetItemC;

                                foreach (IconModel item in TbOpenGrid.Items)
                                {
                                    GridViewItem container = TbOpenGrid.ContainerFromItem(item) as GridViewItem;
                                    if (container != null)
                                    {
                                        VisualStateManager.GoToState(container, item == TbOpenGrid.SelectedItem ? "Pressed" : "Normal", true);
                                    }
                                }
                            }
                        }
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
                    case EVENT_OBJECT_NAMECHANGED:
                        try
                        {
                            IconModel icon = TbIconCollection.First(param => param.Id == hwnd);
                            icon.IconName = windowName;
                        }
                        catch
                        {
                            if (windowName != "Quick settings" && windowName != "Notification Center")
                            {
                                if (!TbIconCollection.Any(item => item.Id == hwnd))
                                {
                                    SoftwareBitmapSource bmpSource = m_bmpHelper.GetXamlBitmapFromGdiBitmapAsync(m_appHelper.GetUwpOrWin32Icon(hwnd, 32)).Result;
                                    TbIconCollection.Add(new IconModel { IconName = windowName, Id = hwnd, AppIcon = bmpSource });
                                }
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
                        if (windowName == "Start")
                        {
                            StartButton.IsChecked = false;
                        }
                        else if (windowName == "Search")
                        {

                        }
                        else if (windowName == "Quick settings")
                        {
                            SystemControls.IsChecked = false;
                        }
                        else if (windowName == "Notification Center" || windowName == "Windows Shell Experience Host")
                        {
                            ActionCenter.IsChecked = false;
                        }
                        else
                        {
                            indexedWindows.Remove(hwnd);
                            try
                            {
                                TbIconCollection.Remove(TbIconCollection.First(param => param.Id == hwnd));
                            }
                            catch
                            {
                                Debug.WriteLine("[-] WinEventHook EOC: Value not found in list.");
                            }
                        }
                        Debug.WriteLine("Window cloaked: " + windowName + " | Handle: " + hwnd);
                        break;
                    case EVENT_OBJECT_UNCLOAKED:
                        if (windowName == "Start")
                        {
                            StartButton.IsChecked = true;
                        }
                        else if (windowName == "Search")
                        {

                        }
                        else if (windowName == "Quick settings")
                        {
                            SystemControls.IsChecked = true;
                        }
                        else if (windowName == "Notification Center")
                        {
                            ActionCenter.IsChecked = true;
                        }
                        else
                        {
                            indexedWindows.Add(hwnd);
                            SoftwareBitmapSource bmpSource = m_bmpHelper.GetXamlBitmapFromGdiBitmapAsync(m_appHelper.GetUwpOrWin32Icon(hwnd, 32)).Result;
                            TbIconCollection.Add(new IconModel { IconName = windowName, Id = hwnd, AppIcon = bmpSource });
                        }
                        Debug.WriteLine("Window uncloaked: " + windowName + " | Handle: " + hwnd);
                        break;
                }
            }
        }

        private bool EnumWindowsCallbackMethod(IntPtr hwnd, IntPtr lParam)
        {
            try
            {
                if (isUserWindow(hwnd)) 
                { 
                    indexedWindows.Add(hwnd);
                    SoftwareBitmapSource bmpSource = m_bmpHelper.GetXamlBitmapFromGdiBitmapAsync(m_appHelper.GetUwpOrWin32Icon(hwnd, 32)).Result;
                    TbIconCollection.Add(new IconModel { IconName = m_appHelper.GetWindowTitle(hwnd), Id = hwnd, AppIcon = bmpSource }); 
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
            //IconRightFlyout.ShowAt((FrameworkElement)sender);
        }
    }
}