using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            base.OnNavigatedTo(e);

            if (_colorChange != null && _colorChange.Accepted)
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
    }
}
