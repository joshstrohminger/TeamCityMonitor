using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using BlinkStickUniversal;
using Interfaces;
using Microsoft.Toolkit.Uwp.Helpers;
using MVVM;
using TeamCityMonitor.ViewModels;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupPage : ILinearNavigator
    {
        public IBlinkStick Device { get; private set; }

        private readonly ISetupViewModel _viewModel;
        private ILabeledColor _colorTarget;

        public IRelayCommand GoBack { get; }

        public SetupPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            if (ViewHelper.IsIot)
            {
                GoBack = new RelayCommand(NavigateBack);
            }
            _viewModel = new SetupViewModel(this);
            DataContext = _viewModel;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs backRequestedEventArgs)
        {
            if (!backRequestedEventArgs.Handled && Frame.CanGoBack)
            {
                backRequestedEventArgs.Handled = true;
                NavigateBack();
            }
        }

        private void NavigateBack()
        {
            Frame.GoBack();
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
                Device = (IBlinkStick) e.Parameter ?? throw new ArgumentNullException(nameof(e.Parameter));
            }
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
                if (labeledColor.Color == Colors.Black)
                {
                    MyColorPicker.Color = labeledColor.Color;
                }
                else
                {
                    var hsv = labeledColor.Color.ToHsv();
                    hsv.V = _viewModel.Brightness / 100; // apply the brightness
                    MyColorPicker.Color = hsv.ToArgb();
                }
            }
        }

        private async void ColorAccepted_OnClick(object sender, RoutedEventArgs e)
        {
            var dimmedColor = MyColorPicker.Color;
            if (dimmedColor == Colors.Black)
            {
                _colorTarget.Color = dimmedColor;
            }
            else
            {
                var hsv = dimmedColor.ToHsv();
                _viewModel.Brightness = hsv.V * 100;
                hsv.V = 1;
                _colorTarget.Color = hsv.ToArgb();
            }
            ColorFlyout.Hide();
            await Device.SetColorAsync(dimmedColor);
        }

        private void ColorFlyout_OnClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            _colorTarget = null;
        }

        public bool GoForward()
        {
            return Frame.Navigate(typeof(MonitorPage), _viewModel);
        }
    }
}
