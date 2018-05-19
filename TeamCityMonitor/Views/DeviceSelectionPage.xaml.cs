using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using BlinkStickUniversal;
using DesignData;
using Microsoft.Toolkit.Uwp.UI.Extensions;
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
                SelectedDevice?.BlinkAsync(Colors.White, 4, 100);
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
            OpenDevice = new RelayCommand(async () => await ExecuteOpenDeviceAsync(), CanExecuteOpenDevice);
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

            var devices = (await BlinkStick.FindAllAsync()).ToList<IBlinkStick>();
#if !ARM
            devices.Add(new BlinkStickSimulator());
#endif
            foreach (var device in devices)
            {
                if (await device.OpenDeviceAsync())
                {
                    Devices.Add(device);
                }
            }

            SelectedDevice = Devices.FirstOrDefault();
        }

        private bool CanExecuteOpenDevice()
        {
            return SelectedDevice != null;
        }

        private async Task ExecuteOpenDeviceAsync()
        {
            if (!CanExecuteOpenDevice()) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(SetupPage), SelectedDevice));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            ApplicationViewExtensions.SetTitle(this, "Select Device");
            base.OnNavigatedTo(e);
            ExecuteRefreshDevices();

            if (SelectedDevice != null && AutoRun && e.NavigationMode == NavigationMode.New)
            {
                await ExecuteOpenDeviceAsync();
            }
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
