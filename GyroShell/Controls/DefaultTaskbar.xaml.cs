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
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.ViewModels;
using System.Reflection.Metadata;

namespace GyroShell.Controls
{
    public sealed partial class DefaultTaskbar : Page
    {
        private IEnvironmentInfoService m_envService;
        private ISettingsService m_appSettings;

        private IAppHelperService m_appHelper;
        private IBitmapHelperService m_bmpHelper;

        private ITaskbarManagerService m_tbManager;

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

            TbIconCollection = new ObservableCollection<IconModel>();

            BarBorder.Background = new SolidColorBrush(Color.FromArgb(255, 66, 63, 74));

            callback = WinEventCallback;
            GetCurrentWindows();
            RegisterWinEventHook();

            m_tbManager.NotifyWinlogonShowShell();
        }

        public DefaultTaskbarViewModel ViewModel => (DefaultTaskbarViewModel)this.DataContext;

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
                        break;
                    case "Desktop":
                        foreach (IntPtr handle in indexedWindows)
                        {
                            ShowWindow(handle, SW_MINIMIZE);
                        }
                        break;
                }
            }
        }

        private void MainShellGrid_Loaded(object sender, RoutedEventArgs e)
        {
            App.startupScreen.Close();
        }

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