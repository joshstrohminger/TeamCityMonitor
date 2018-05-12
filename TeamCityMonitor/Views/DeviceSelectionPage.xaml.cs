using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using BlinkStickUniversal;
using DesignData;
using Interfaces;
using MVVM;
using MVVM.Annotations;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceSelectionPage : INotifyPropertyChanged
    {
        private IBlinkStick _selectedDevice;
        private bool _autoRun = App.LocalSettings.AutoRun;

        public IBlinkStick SelectedDevice
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

        public bool AutoRun
        {
            get => _autoRun;
            set => App.LocalSettings.AutoRun = _autoRun = value;
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
#if ARM
            var devices = await BlinkStick.FindAllAsync();
#else
            var devices = new[] {new BlinkStickSimulator()};
#endif
            foreach (var device in devices)
            {
                if (await device.OpenDeviceAsync())
                {
                    Devices.Add(device);
                }
            }

            SelectedDevice = Devices.FirstOrDefault();
            if (SelectedDevice != null && AutoRun)
            {
                ExecuteOpenDevice();
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

        public ObservableCollection<IBlinkStick> Devices { get; } = new ObservableCollection<IBlinkStick>();
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
