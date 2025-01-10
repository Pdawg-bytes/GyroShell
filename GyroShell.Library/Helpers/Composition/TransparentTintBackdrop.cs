using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Runtime.InteropServices;
using WinRT;

using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Library.Helpers.Composition
{
    public partial class TransparentTintBackdrop : Microsoft.UI.Xaml.Media.SystemBackdrop
    {
        private readonly Windows.UI.Composition.Compositor _compositor;
        private Windows.UI.Composition.CompositionColorBrush _brush;
        private IntPtr _backgroundBrush = IntPtr.Zero;
        private Windows.UI.Color _tintColor;

        public TransparentTintBackdrop(Windows.UI.Composition.Compositor compositor, Windows.UI.Color tintColor)
        {
            _compositor = compositor ?? throw new ArgumentNullException(nameof(compositor));
            TintColor = tintColor;
        }

        public Windows.UI.Color TintColor
        {
            get => _tintColor;
            set
            {
                _tintColor = value;
                if (_brush != null)
                {
                    _brush.Color = value;
                }
            }
        }

        protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
        {
            base.OnDefaultSystemBackdropConfigurationChanged(target, xamlRoot);
        }

        protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            _brush = _compositor.CreateColorBrush(TintColor);
            connectedTarget.SystemBackdrop = _brush;

            var inspectable = connectedTarget.As<IInspectable>();
            var xamlSource = DesktopWindowXamlSource.FromAbi(inspectable.ThisPtr);
            var hWnd = xamlSource.SiteBridge.SiteView.EnvironmentView.AppWindowId.Value;

            ConfigureDwm(hWnd);
            ClearBackground((nint)hWnd, GetDC((IntPtr)hWnd));

            base.OnTargetConnected(connectedTarget, xamlRoot);
        }

        protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
        {
            if (_backgroundBrush != IntPtr.Zero)
                DeleteObject(_backgroundBrush);
            _backgroundBrush = IntPtr.Zero;

            disconnectedTarget.SystemBackdrop = null;
            _brush?.Dispose();
            _brush = null;

            base.OnTargetDisconnected(disconnectedTarget);
        }

        private static void ConfigureDwm(ulong hWnd)
        {
            IntPtr _windowHandle = (IntPtr)hWnd;

            MARGINS margins = new() { };
            DwmExtendFrameIntoClientArea(_windowHandle, ref margins);

            DWM_BLURBEHIND blur = new()
            {
                dwFlags = 3,
                fEnable = true,
                hRgnBlur = CreateRectRgn(-2, -2, -1, -1),
                fTransitionOnMaximized = true,
            };
            DwmEnableBlurBehindWindow(_windowHandle, ref blur);
        }

        private bool ClearBackground(IntPtr hWnd, nint hdc)
        {
            if (GetClientRect(hWnd, out var rect))
            {
                if (_backgroundBrush == IntPtr.Zero)
                    _backgroundBrush = CreateSolidBrush(0);
                FillRect(hdc, ref rect, _backgroundBrush);
                return true;
            }
            return false;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int FillRect(IntPtr hdc, ref RECT lprc, IntPtr hbr);
    }
}