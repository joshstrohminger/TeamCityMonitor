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
            
            _vm = (IColorChangeViewModel) e.Parameter ?? throw new ArgumentNullException(nameof(e.Parameter));
            MyColorPicker.Color = _vm.WorkingColor;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void Accept_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.ChangeColor(MyColorPicker.Color);
            Frame.GoBack();
        }
    }
}
