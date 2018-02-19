using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using BlinkStickUniversal;
using Interfaces;
using TeamCityMonitor.ViewModels;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupPage
    {
        public BlinkStick Device { get; private set; }

        private ISetupViewModel _viewModel;
        private Rectangle _colorTarget;

        public SetupPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs backRequestedEventArgs)
        {
            if (!backRequestedEventArgs.Handled && Frame.CanGoBack)
            {
                backRequestedEventArgs.Handled = true;
                _viewModel = null;
                DataContext = null;
                Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            base.OnNavigatedTo(e);
            
            if (e.NavigationMode == NavigationMode.Back)
            {
                //todo handle navigating back from the monitor page
            }
            else if (e.NavigationMode == NavigationMode.New)
            {
                Device = (BlinkStick) e.Parameter ?? throw new ArgumentNullException(nameof(e.Parameter));
                _viewModel = new SetupViewModel();
                DataContext = _viewModel;
            }
        }

        private void Monitor_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MonitorPage));
        }

        private void ColorFlyout_OnOpening(object sender, object e)
        {
            //nothing to do here anymore?
        }

        private void ColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            if (button.Content is Rectangle rectangle && rectangle.Fill is SolidColorBrush brush)
            {
                _colorTarget = rectangle;
                var brightened = HslColor.FromRgbColor(new RgbColor(brush.Color.R, brush.Color.G, brush.Color.B));
                var hsl = new HslColor(brightened.Hue, brightened.Saturation, _viewModel.Brightness / 100d);
                var rgb = hsl.ToRgbColor();
                var color = new Color
                {
                    R = rgb.R,
                    G = rgb.G,
                    B = rgb.B
                };
                MyColorPicker.Color = color;
            }
        }

        private async void ColorAccepted_OnClick(object sender, RoutedEventArgs e)
        {
            var color = new RgbColor(MyColorPicker.Color.R, MyColorPicker.Color.G, MyColorPicker.Color.B);
            var hsl = HslColor.FromRgbColor(color);
            var brightened = new HslColor(hsl.Hue, hsl.Saturation, 1).ToRgbColor();
            _colorTarget.Fill = new SolidColorBrush(new Color{R = brightened.R, G = brightened.G, B = brightened.B});
            _viewModel.Brightness = hsl.Lightness * 100;
            ColorFlyout.Hide();
            await Device.SetColorAsync(color);
        }

        private void ColorFlyout_OnClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            _colorTarget = null;
        }
    }
}
