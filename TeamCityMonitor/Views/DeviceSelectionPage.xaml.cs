using System;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BlinkStickDotNet;
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

        private void ExecuteRefreshDevices()
        {
            Devices.Clear();
            foreach (var device in BlinkStick.FindAll())
            {
                Devices.Add(device);
            }
        }

        private bool CanExecuteOpenDevice(BlinkStick blinkStick)
        {
            return blinkStick != null;
        }

        private void ExecuteOpenDevice(BlinkStick blinkStick)
        {
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
