using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using BlinkStickUniversal;
using Interfaces;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using MVVM;
using TeamCityMonitor.ViewModels;

namespace TeamCityMonitor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupPage : ILinearNavigator
    {
        private readonly ISetupViewModel _viewModel;
        private ILabeledColor _colorTarget;

        public IRelayCommand GoBack { get; }
        public IRelayCommand SaveAsDefault { get; }

        public SetupPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            if (ViewHelper.IsIot)
            {
                GoBack = new RelayCommand(NavigateBack);
            }
            SaveAsDefault = new RelayCommand(async () => await ExecuteSaveAsDefault());
            _viewModel = new SetupViewModel(this);
            DataContext = _viewModel;
        }

        private async Task ExecuteSaveAsDefault()
        {
            App.LocalSettings.Host = _viewModel.Host;
            await App.LocalSettings.SaveBuilds(_viewModel.Builds.OfType<BuildViewModel>().ToArray());
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs backRequestedEventArgs)
        {
            if (!backRequestedEventArgs.Handled && Frame.CanGoBack)
            {
                backRequestedEventArgs.Handled = true;
                NavigateBack();
            }
        }

        private void NavigateBack()
        {
            Frame.GoBack();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            base.OnNavigatedTo(e);
            ApplicationViewExtensions.SetTitle(this, "Setup");

            if (e.NavigationMode == NavigationMode.Back)
            {
                //todo handle navigating back from the monitor page
            }
            else if (e.NavigationMode == NavigationMode.New)
            {
                _viewModel.Device = (IBlinkStick) e.Parameter ?? throw new ArgumentNullException(nameof(e.Parameter));

                _viewModel.Host = App.LocalSettings.Host;
                _viewModel.Brightness = App.LocalSettings.Brightness;
                _viewModel.Builds.Clear();
                foreach (var build in await App.LocalSettings.GetSavedBuilds())
                {
                    _viewModel.Builds.Add(build);
                }

                if (_viewModel.Builds.Count == 0)
                {
                    _viewModel.AddBuild.Execute(null);
                }

                if (App.LocalSettings.AutoRun)
                {
                    _viewModel.Monitor.Execute(null);
                }
            }
        }

        private void ColorFlyout_OnOpening(object sender, object e)
        {
            //nothing to do here anymore?
        }

        private void ColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            if (button.DataContext is ILabeledColor labeledColor)
            {
                _colorTarget = labeledColor;
                if (labeledColor.Color == Colors.Black)
                {
                    MyColorPicker.Color = labeledColor.Color;
                }
                else
                {
                    var hsv = labeledColor.Color.ToHsv();
                    hsv.V = _viewModel.Brightness / 100; // apply the brightness
                    MyColorPicker.Color = hsv.ToArgb();
                }
            }
        }

        private async void ColorAccepted_OnClick(object sender, RoutedEventArgs e)
        {
            var dimmedColor = MyColorPicker.Color;
            if (dimmedColor == Colors.Black)
            {
                _colorTarget.Color = dimmedColor;
            }
            else
            {
                var hsv = dimmedColor.ToHsv();
                _viewModel.Brightness = hsv.V * 100;
                hsv.V = 1;
                _colorTarget.Color = hsv.ToArgb();
            }
            ColorFlyout.Hide();
            await _viewModel.Device.SetColorAsync(dimmedColor);
        }

        private void ColorFlyout_OnClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            _colorTarget = null;
        }

        public async void GoForward()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(MonitorPage), _viewModel));
        }
    }
}
