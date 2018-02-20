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
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
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
        private ILabeledColor _colorTarget;

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
            if (button.DataContext is ILabeledColor labeledColor)
            {
                _colorTarget = labeledColor;
                var hsv = Color.FromArgb(255, labeledColor.Color.R, labeledColor.Color.G, labeledColor.Color.B).ToHsv();
                hsv.V = _viewModel.Brightness / 100; // apply the brightness
                MyColorPicker.Color = hsv.ToArgb();
            }
        }

        private async void ColorAccepted_OnClick(object sender, RoutedEventArgs e)
        {
            var dimmedColor = MyColorPicker.Color;
            var hsv = MyColorPicker.Color.ToHsv();
            _viewModel.Brightness = hsv.V * 100;
            hsv.V = 1;
            var brightenedColor = hsv.ToArgb();
            _colorTarget.Color = new RgbColor(brightenedColor.R, brightenedColor.G, brightenedColor.B);
            ColorFlyout.Hide();
            await Device.SetColorAsync(new RgbColor(dimmedColor.R, dimmedColor.G, dimmedColor.B));
        }

        private void ColorFlyout_OnClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            _colorTarget = null;
        }
    }
}
