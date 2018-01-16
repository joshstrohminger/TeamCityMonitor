using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Interfaces;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using ColorHelper = Microsoft.Toolkit.Uwp.Helpers.ColorHelper;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ColorSelectionPage : Page
    {
        private IColorChangeViewModel _vm;

        public ColorSelectionPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs backRequestedEventArgs)
        {
            if (!backRequestedEventArgs.Handled && Frame.CanGoBack)
            {
                backRequestedEventArgs.Handled = true;
                Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            base.OnNavigatedTo(e);

            if (e.Parameter is IColorChangeViewModel vm)
            {
                _vm = vm;
                _vm.Accepted = false;
                var hsv = _vm.OriginalColor.ToHsv();
                MyColorPicker.Color = ColorHelper.FromHsv(hsv.H, hsv.S, _vm.OriginalBrightness / 100d);
            }
            else
            {
                _vm = null;
                new MessageDialog("Color not passed", "Invalid parameter").ShowAsync();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void Accept_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm != null)
            {
                var hsv = MyColorPicker.Color.ToHsv();
                _vm.NewBrightness = (byte)Math.Round(hsv.V * 100, MidpointRounding.AwayFromZero);
                _vm.NewColor = ColorHelper.FromHsv(hsv.H, hsv.S, 1);
                _vm.Accepted = true;
            }
            
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
