using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MVVM;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonitorPage : Page
    {
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
        }
    }
}
