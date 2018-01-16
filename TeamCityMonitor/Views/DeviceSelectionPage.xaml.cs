using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceSelectionPage : Page
    {
        public DeviceSelectionPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            base.OnNavigatedTo(e);
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SetupPage));
        }
    }
}
