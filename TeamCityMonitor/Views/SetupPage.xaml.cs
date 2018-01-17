using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Interfaces;
using TeamCityMonitor.ViewModels;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        private byte _brightness = 100;
        private IColorChangeViewModel _colorChange;
        public SetupPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            MyBrightness.Text = _brightness.ToString();
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs backRequestedEventArgs)
        {
            if (!backRequestedEventArgs.Handled && Frame.CanGoBack)
            {
                _colorChange = null;
                backRequestedEventArgs.Handled = true;
                Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            base.OnNavigatedTo(e);
            
            if (e.NavigationMode == NavigationMode.Back && _colorChange?.Accepted == true)
            {
                _brightness = _colorChange.NewBrightness;
                MyBrightness.Text = _brightness.ToString();
                MyRectangle.Fill = new SolidColorBrush(_colorChange.NewColor);
            }
            _colorChange = null;
        }

        private void Monitor_OnClick(object sender, RoutedEventArgs e)
        {
            _colorChange = null;
            Frame.Navigate(typeof(MonitorPage));
        }

        private void Color_OnClick(object sender, RoutedEventArgs e)
        {
            if (MyRectangle.Fill is SolidColorBrush brush)
            {
                _colorChange = new ColorChangeViewModel(brush.Color, _brightness);
                Frame.Navigate(typeof(ColorSelectionPage), _colorChange);
            }
        }

        private void ColorFlyout_OnOpening(object sender, object e)
        {
            MyColorPicker.Color = _change.WorkingColor;
        }

        private void ColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var rectangle = (Rectangle) button.Content;
            var brush = (SolidColorBrush) rectangle.Fill;
            _change = new ColorChangeViewModel(brush.Color, (byte) BrightnessSlider.Value)
            {
                Source = rectangle
            };
        }

        private ColorChangeViewModel _change;

        private void ColorAccepted_OnClick(object sender, RoutedEventArgs e)
        {
            _change.ChangeColor(MyColorPicker.Color);
            ColorFlyout.Hide();
        }

        private void ColorFlyout_OnClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            if (_change?.Accepted == true)
            {
                _change.Source.Fill = new SolidColorBrush(_change.NewColor);
                BrightnessSlider.Value = _change.NewBrightness;
            }
            _change = null;
        }
    }
}
