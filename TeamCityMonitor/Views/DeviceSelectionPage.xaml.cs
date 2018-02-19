using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using BlinkStickUniversal;
using MVVM;
using MVVM.Annotations;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceSelectionPage : INotifyPropertyChanged
    {
        private BlinkStick _selectedDevice;

        public BlinkStick SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (ReferenceEquals(_selectedDevice, value)) return;
                _selectedDevice = value;
                OnPropertyChanged();
                OpenDevice.RaiseCanExecuteChanged();
            }
        }

        public DeviceSelectionPage()
        {
            OpenDevice = new RelayCommand(ExecuteOpenDevice, CanExecuteOpenDevice);
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

        private bool CanExecuteOpenDevice()
        {
            return SelectedDevice != null;
        }

        private void ExecuteOpenDevice()
        {
            if (!CanExecuteOpenDevice()) return;
            Frame.Navigate(typeof(SetupPage), SelectedDevice);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            base.OnNavigatedTo(e);
            ExecuteRefreshDevices();
        }

        public IRelayCommand OpenDevice { get; }
        public IRelayCommand RefreshDevices { get; }

        public ObservableCollection<BlinkStick> Devices { get; } = new ObservableCollection<BlinkStick>();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
