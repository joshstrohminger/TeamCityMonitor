using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Api;
using Api.Models;
using BlinkStickUniversal;
using Interfaces;
using Microsoft.Toolkit.Uwp.Helpers;
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
        private int _leds = 8;
        private readonly ISetupViewModel _setup;
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
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_brightness != value)
                {
                    _brightness = value;
                    _setup.Brightness = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Construction

        public MonitorViewModel(ISetupViewModel setup)
        {
            _setup = setup ?? throw new ArgumentNullException(nameof(setup));

            Device = setup.Device;
            Host = setup.Host;
            Brightness = setup.Brightness;
            BuildMonitors = new ReadOnlyCollection<IBuildMonitor>(setup.Builds.Select((build,i) =>
                    new BuildMonitor(new BuildStatusViewModel(build),
                        TeamCityApiFactory.Create(Host, build.Id, i),
                        build))
                .ToList<IBuildMonitor>());

            // todo make this dynamic instead of a special case
            if (BuildMonitors.Count == 2)
            {
                // assume 8 LEDs
                setup.Builds[0].AllLedIndexes = new[] {7, 0, 1, 2};
                setup.Builds[0].FirstRunningQueuedLedIndex = 2;
                setup.Builds[0].SecondRunningQueuedledIndex = 1;
                setup.Builds[1].AllLedIndexes = new[] {3, 4, 5, 6};
                setup.Builds[1].FirstRunningQueuedLedIndex = 3;
                setup.Builds[1].SecondRunningQueuedledIndex = 4;
            }

            Refresh = new RelayCommand(async () => await ExecuteRefreshAsync(true));

            _autoRefreshTimer = new DispatcherTimer {Interval = TimeSpan.FromMinutes(1)};
            _autoRefreshTimer.Tick += async (sender, o) => await ExecuteRefreshAsync(false);
            _autoRefreshTimer.Start();

            _refreshAgeTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _refreshAgeTimer.Tick += RefreshAgeTimerOnTick;
            _refreshAgeTimer.Start();
        }

        #endregion

        #region Methods

        private void RefreshAgeTimerOnTick(object sender, object e)
        {
            foreach (var monitor in BuildMonitors)
            {
                monitor.Status.RefreshTimeDependentProperties();
            }

            if (_lastUpdatedTime.HasValue)
            {
                LastUpdated = "Updated  " + (DateTime.Now - _lastUpdatedTime.Value).ToAgeString();
            }
        }

        private async Task ExecuteRefreshAsync(bool manualRefresh)
        {
            if (manualRefresh && AutoRefresh)
            {
                // restart the timer since we just manually refreshed
                _autoRefreshTimer.Stop();
                _autoRefreshTimer.Start();
            }
            
            var onColors = new Color[_leds];
            var offColors = new Color[_leds];
            var blink = false;

            foreach (var buildMonitor in BuildMonitors)
            {
                foreach (var i in buildMonitor.Setup.AllLedIndexes)
                {
                    onColors[i] = buildMonitor.Setup.ApiCall;
                }
            }
            Scale(ref onColors);
            await Device.SetColorsAsync(onColors);

            var updates = BuildMonitors.AsParallel()
                .Select(buildMonitor => (buildMonitor: buildMonitor, summary: buildMonitor.Api.RefreshAsync().Result))
                .ToList();

            foreach (var update in updates)
            {
                var status = update.buildMonitor.Status;
                var setup = update.buildMonitor.Setup;
                var summary = update.summary;
                var previousStatus = status.OverallStatus;
                status.Update(summary);
                var statusChanged = status.OverallStatus != previousStatus;
                blink |= statusChanged;
                var overallStatusColor = update.buildMonitor.GetOverallStatusColor();
                foreach (var i in setup.AllLedIndexes)
                {
                    onColors[i] = overallStatusColor;
                    offColors[i] = statusChanged ? setup.Idle : overallStatusColor;
                }

                // Don't setup queued or running colors if there is an api error
                if(status.IsApiError) continue;

                var nextIndex = setup.FirstRunningQueuedLedIndex;
                if (status.IsQueued)
                {
                    onColors[nextIndex] = setup.Queued;
                    offColors[nextIndex] = statusChanged ? setup.Idle : setup.Queued;
                    nextIndex = setup.SecondRunningQueuedledIndex;
                }
                if (status.IsRunning)
                {
                    onColors[nextIndex] = setup.Running;
                    offColors[nextIndex] = statusChanged ? setup.Idle : setup.Running;
                }
            }

            Scale(ref onColors);
            if (blink)
            {
                Scale(ref offColors);
                await Device.BlinkAsync(onColors, offColors, 5, 200);
            }
            await Device.SetColorsAsync(onColors);

            _lastUpdatedTime = DateTime.Now;
        }

        private void Scale(ref Color[] colors)
        {
            for (var i = 0; i < colors.Length; i++)
            {
                if(colors[i] == Colors.Black) continue;
                var hsv = colors[i].ToHsv();
                hsv.V = Brightness / 100; // apply the brightness which is 0 to 100 to V which is 0 to 1
                colors[i] = hsv.ToArgb();
            }
        }

        public void Dispose()
        {
            _refreshAgeTimer.Stop();
            _autoRefreshTimer.Stop();
        }

        #endregion
    }
}
