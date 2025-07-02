using GyroShell.Library.Services.Environment;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using WinRT;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using WinRT.Interop;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Library.Helpers.Window
{
    public class ShellWindow : Microsoft.UI.Xaml.Window
    {
        protected readonly int Width;
        protected readonly int Height;
        protected readonly int X;
        protected readonly int Y;

        protected readonly bool CustomTransparency;
        protected readonly bool IsRound;

        protected readonly ISettingsService SettingsService;

        protected IntPtr WindowHandle;

        protected readonly SystemBackdrop CustomBackdrop;
        private WindowsSystemDispatcherQueueHelper _queueHelper;
        private SystemBackdropConfiguration _backdropConfig;
        private MicaController _mica;
        private DesktopAcrylicController _acrylic;

        private bool _themeHooked = false;

        public ShellWindow(ISettingsService settingsProvider,
                           int width, int height, int x = 0, int y = 0,
                           bool dockBottom = false, bool round = false,
                           bool customTransparency = true, SystemBackdrop customBackdrop = null)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;

            if (dockBottom)
            {
                X = 0;
                Y = GetSystemMetrics(SM_CYSCREEN) - height;
            }
            else
            {
                X = x;
                Y = y;
            }

            IsRound = round;
            CustomTransparency = customTransparency;
            CustomBackdrop = customBackdrop;

            SettingsService = settingsProvider;

            InitializeShellWindow();
        }

        private void InitializeShellWindow()
        {
            WindowHandle = WindowNative.GetWindowHandle(this);


            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsAlwaysOnTop = true;
                presenter.IsResizable = false;
                presenter.SetBorderAndTitleBar(false, false);
            }
            AppWindow.SetPresenter(AppWindowPresenterKind.Default);

            if (!IsRound && Environment.OSVersion.Version.Build >= 22000)
            {
                DWMWINDOWATTRIBUTE attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                int preference = (int)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
                DwmSetWindowAttribute(WindowHandle, attribute, ref preference, sizeof(uint));
            }

            SetWindowLongPtr(WindowHandle, GWL_EXSTYLE, GetWindowLongPtr(WindowHandle, GWL_EXSTYLE) | WS_EX_TOOLWINDOW);

            long style = GetWindowLongPtr(WindowHandle, GWL_STYLE).ToInt64();
            style &= ~(WS_BORDER | WS_DLGFRAME | WS_THICKFRAME);
            SetWindowLongPtr(WindowHandle, GWL_STYLE, new IntPtr(style));


            AppWindow.Resize(new SizeInt32 { Width = Width, Height = Height });
            AppWindow.Move(new PointInt32 { X = this.X, Y = this.Y });
            AppWindow.MoveInZOrderAtTop();

            SetupBackdrop();
            this.Closed += (_, _) => CleanupBackdrop();
            this.Activated += (_, _) =>
            {
                if (!_themeHooked && this.Content is FrameworkElement fe)
                {
                    _themeHooked = true;
                    SetBackdropTheme();
                    fe.ActualThemeChanged += (_, _) => SetBackdropTheme();
                }

                if (_backdropConfig is not null)
                    _backdropConfig.IsInputActive = true;
            };
        }

        private void SetupBackdrop()
        {
            _queueHelper = new();
            _queueHelper.EnsureWindowsSystemDispatcherQueueController();

            _backdropConfig = new SystemBackdropConfiguration
            {
                IsInputActive = true
            };

            SetBackdropTheme();

            if (CustomBackdrop is not null)
            {
                SystemBackdrop = CustomBackdrop;
                return;
            }

            int type = SettingsService.TransparencyType;
            Color tint = CustomTransparency ?
                Color.FromArgb(
                    SettingsService.AlphaTint,
                    SettingsService.RedTint,
                    SettingsService.GreenTint,
                    SettingsService.BlueTint
                )
                : GetThemeBasedTint();

            bool isMica = Environment.OSVersion.Version.Build >= 22000 && (type == 0 || type == 1);
            bool success = isMica
                ? TryApplyMica((MicaKind)(type == 0 ? 0 : 1), tint)
                : TryApplyAcrylic(tint);

            if (!success && type != 2)
                TryApplyAcrylic(tint);
        }

        private bool TryApplyMica(MicaKind kind, Color tint)
        {
            if (!MicaController.IsSupported()) return false;

            _mica = new MicaController { Kind = kind };

            if (CustomTransparency)
            {
                _mica.TintColor = tint;
                _mica.TintOpacity = SettingsService.TintOpacity;
            }

            _mica.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            _mica.SetSystemBackdropConfiguration(_backdropConfig);
            return true;
        }

        private bool TryApplyAcrylic(Color tint)
        {
            if (!DesktopAcrylicController.IsSupported()) return false;

            _acrylic = new DesktopAcrylicController
            {
                TintColor = tint,
                TintOpacity = SettingsService.TintOpacity,
                LuminosityOpacity = SettingsService.LuminosityOpacity
            };

            _acrylic.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            _acrylic.SetSystemBackdropConfiguration(_backdropConfig);
            return true;
        }

        private void SetBackdropTheme()
        {
            if (this.Content is not FrameworkElement content) return;

            _backdropConfig.Theme = content.ActualTheme switch
            {
                ElementTheme.Dark => SystemBackdropTheme.Dark,
                ElementTheme.Light => SystemBackdropTheme.Light,
                _ => SystemBackdropTheme.Default
            };

            if (!CustomTransparency)
            {
                Color fallbackTint = GetThemeBasedTint();

                if (_acrylic is not null)
                    _acrylic.TintColor = fallbackTint;

                if (_mica is not null)
                    _mica.TintColor = fallbackTint;
            }
        }

        private Color GetThemeBasedTint()
        {
            return _backdropConfig.Theme switch
            {
                SystemBackdropTheme.Dark or SystemBackdropTheme.Default => Color.FromArgb(255, 32, 32, 32),
                SystemBackdropTheme.Light => Color.FromArgb(255, 232, 232, 232),
                _ => Color.FromArgb(255, 0, 0, 0)
            };
        }

        private void CleanupBackdrop()
        {
            _mica?.Dispose();
            _acrylic?.Dispose();
            _mica = null;
            _acrylic = null;
            _backdropConfig = null;
        }
    }
}