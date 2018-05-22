using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Gpio;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using TeamCityMonitor.ViewModels;

namespace TeamCityMonitor
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        public static class LocalSettings
        {
            private static readonly IPropertySet Values = ApplicationData.Current.LocalSettings.Values;

            public static string Host
            {
                get => Values.TryGetValue(nameof(Host), out var value) ? (string) value : null;
                set => Values[nameof(Host)] = value;
            }

            public static double Brightness
            {
                get => Values.TryGetValue(nameof(Brightness), out var value) ? (double) value : 100;
                set => Values[nameof(Brightness)] = value;
            }

            public static bool AutoRun
            {
                get => Values.TryGetValue(nameof(AutoRun), out var value) && (bool) value;
                set => Values[nameof(AutoRun)] = value;
            }

            public static async Task<BuildViewModel[]> GetSavedBuilds()
            {
                try
                {
                    var file = await ApplicationData.Current.LocalFolder.GetFileAsync("Builds.bin");
                    var serializer = new DataContractSerializer(typeof(BuildViewModel[]), new[] { typeof(LabeledColor) });
                    using (var stream = await file.OpenStreamForReadAsync())
                    {
                        return (BuildViewModel[]) serializer.ReadObject(stream);
                    }
                }
                catch (Exception e) when (e is FileNotFoundException || e is XmlException)
                {
                    return new BuildViewModel[0];
                }
            }
            
            public static async Task SaveBuilds(BuildViewModel[] builds)
            { 
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Builds.bin", CreationCollisionOption.ReplaceExisting);
                var serializer = new DataContractSerializer(typeof(BuildViewModel[]), new []{ typeof(LabeledColor) });
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    serializer.WriteObject(stream, builds);
                }
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(Views.DeviceSelectionPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
            

            var gpio = GpioController.GetDefault();
            if (gpio is null) return;

            _button1Listener = new ButtonListener(gpio, 12);
            _button2Listener = new ButtonListener(gpio, 16);
        }

        private ButtonListener _button1Listener;
        private ButtonListener _button2Listener;

        public event EventHandler Button1Pressed
        {
            add
            {
                if (_button1Listener != null)
                {
                    _button1Listener.Pressed += value;
                }
            }
            remove
            {
                if (_button1Listener != null)
                {
                    _button1Listener.Pressed -= value;
                }
            }
        }
        public event EventHandler Button2Pressed
        {
            add
            {
                if (_button2Listener != null)
                {
                    _button2Listener.Pressed += value;
                }
            }
            remove
            {
                if (_button2Listener != null)
                {
                    _button2Listener.Pressed -= value;
                }
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
