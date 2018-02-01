using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BlinkStickUniversal;
using MVVM;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceSelectionPage : Page
    {
        public DeviceSelectionPage()
        {
            OpenDevice = new RelayCommand<BlinkStick>(ExecuteOpenDevice, CanExecuteOpenDevice);
            RefreshDevices = new RelayCommand(ExecuteRefreshDevices);
            InitializeComponent();
        }

        private async void ExecuteRefreshDevices()
        {
            foreach (var device in Devices)
            {
                device.CloseDevice();
            }
            Devices.Clear();
            var devices = await BlinkStick.FindAllAsync();
            foreach (var device in devices)
            {
                if (await device.OpenDeviceAsync())
                {
                    Devices.Add(device);
                }
            }
        }

        private bool CanExecuteOpenDevice(BlinkStick blinkStick)
        {
            return blinkStick?.Connected == true;
        }

        private void ExecuteOpenDevice(BlinkStick blinkStick)
        {
            if (!CanExecuteOpenDevice(blinkStick)) return;
            Frame.Navigate(typeof(SetupPage), blinkStick);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            base.OnNavigatedTo(e);
        }

        public IRelayCommand OpenDevice { get; }
        public IRelayCommand RefreshDevices { get; }

        public ObservableCollection<BlinkStick> Devices { get; } = new ObservableCollection<BlinkStick>();
    }
}
