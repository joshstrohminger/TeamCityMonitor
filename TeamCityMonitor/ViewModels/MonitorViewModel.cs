using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Api;
using BlinkStickUniversal;
using Interfaces;
using MVVM;
using TeamCityMonitor.Interfaces;
using TeamCityMonitor.Models;
using TeamCityMonitor.Views;

namespace TeamCityMonitor.ViewModels
{
    public class MonitorViewModel : ObservableObject, IMonitorViewModel
    {
        #region Fields

        private bool _autoRefresh = true;
        private string _lastUpdated = "never";
        private double _brightness;
        private DateTime? _lastUpdatedTime;
        private readonly DispatcherTimer _autoRefreshTimer;
        private readonly DispatcherTimer _refreshAgeTimer;

        #endregion

        #region Properties

        public IBlinkStick Device { get; }
        public string Host { get; }
        public IReadOnlyCollection<IBuildMonitor> BuildMonitors { get; }
        public IRelayCommand Refresh { get; }

        public string LastUpdated
        {
            get => _lastUpdated;
            private set => UpdateOnPropertyChanged(ref _lastUpdated, value);
        }

        public bool AutoRefresh
        {
            get => _autoRefresh;
            set
            {
                if (_autoRefresh != value)
                {
                    _autoRefresh = value;
                    if (AutoRefresh)
                    {
                        _autoRefreshTimer.Start();
                    }
                    else
                    {
                        _autoRefreshTimer.Stop();
                    }
                    OnPropertyChanged();
                }
            }
        }

        public double Brightness
        {
            get => _brightness;
            set => UpdateOnPropertyChanged(ref _brightness, value);
        }

        #endregion

        public MonitorViewModel(ISetupViewModel setup)
        {
            if (setup is null) throw new ArgumentNullException(nameof(setup));

            Device = setup.Device;
            Host = setup.Host;
            Brightness = setup.Brightness;
            BuildMonitors = new ReadOnlyCollection<IBuildMonitor>(setup.Builds.Select(build =>
                    new BuildMonitor(new BuildStatusViewModel(build), new TeamCityApi(setup.Host, build.Id), build))
                .ToList<IBuildMonitor>());
            Refresh = new RelayCommand(async () => await ExecuteRefreshAsync());

            _autoRefreshTimer = new DispatcherTimer {Interval = TimeSpan.FromMinutes(1)};
            _autoRefreshTimer.Tick += async (sender, o) => await ExecuteRefreshAsync();
            _autoRefreshTimer.Start();

            _refreshAgeTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _refreshAgeTimer.Tick += RefreshAgeTimerOnTick;
            _refreshAgeTimer.Start();

            ExecuteRefreshAsync();
        }

        private void RefreshAgeTimerOnTick(object sender, object e)
        {
            foreach (var monitor in BuildMonitors)
            {
                monitor.Status.RefreshTimeDependentProperties();
            }

            if (_lastUpdatedTime.HasValue)
            {
                LastUpdated = $"Updated  " + (DateTime.Now - _lastUpdatedTime.Value).ToAgeString();
            }
        }

        private async Task ExecuteRefreshAsync()
        {
            foreach (var buildMonitor in BuildMonitors)
            {
                var summary = await buildMonitor.Api.RefreshAsync();
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => buildMonitor.Status.Update(summary));
                // todo should colors be updated here?
            }

            _lastUpdatedTime = DateTime.Now;
        }

        public void Dispose()
        {
            _refreshAgeTimer.Stop();
            _autoRefreshTimer.Stop();
        }
    }
}
