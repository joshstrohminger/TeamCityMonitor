using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
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
                Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            base.OnNavigatedTo(e);
        }

        private void Monitor_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MonitorPage));
        }

        private void Color_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ColorSelectionPage));
        }
    }
}
