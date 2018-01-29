using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BlinkStickCore;
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
            ushort vendorId = 0x20A0;
            ushort productId = 0x41E5;
            ushort usagePage = 0xFF00;
            ushort usageId = 0x0001;

            // Create a selector that gets a HID device using VID/PID and a 
            // VendorDefined usage
            string selector = HidDevice.GetDeviceSelector(usagePage, usageId,
                vendorId, productId);
            
            // Enumerate devices using the selector
            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Count > 0)
            {
                var d = devices.ElementAt(0);
                var i = DeviceAccessInformation.CreateFromId(d.Id);
                // Open the target HID device
                HidDevice device = await HidDevice.FromIdAsync(d.Id, FileAccessMode.ReadWrite);
                if (device == null)
                {
                    //DeviceAccessInformation.CurrentStatus()
                    var s = i.CurrentStatus;

                    i = DeviceAccessInformation.CreateFromDeviceClassId(new Guid("{745A17A0-74D3-11D0-B6FE-00A0C90F57DA}"));
                    s = i.CurrentStatus;
                }

                // At this point the device is available to communicate with
                // So we can send/receive HID reports from it or 
                // query it for control descriptions
                device?.Dispose();
            }

            //Devices.Clear();
            //foreach (var device in BlinkStick.FindAll())
            //{
            //    Devices.Add(device);
            //}
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
