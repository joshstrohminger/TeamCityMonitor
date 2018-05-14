using System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BlinkStickUniversal;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonitorPage : Page
    {
        public ISetupViewModel ViewModel { get; private set; }

        public IRelayCommand GoBack { get; }

        public MonitorPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            if (ViewHelper.IsIot)
            {
                GoBack = new RelayCommand(NavigateBack);
            }
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

            ViewModel = (ISetupViewModel)e.Parameter ?? throw new ArgumentNullException(nameof(e.Parameter));
            ViewModel.Device.SetColorsAsync(1, new byte[]
            {
                255,0,0,
                0,255,0,
                0,0,255,
                255,255,0,
                255,0,255,
                0,255,255,
                255,255,255,
                255,100,5
            });
            DataContext = ViewModel;
        }
    }
}
