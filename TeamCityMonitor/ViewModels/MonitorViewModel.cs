using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Api;
using BlinkStickUniversal;
using Interfaces;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;
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
        private int _leds = -1;
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

        public MonitorViewModel(ISetupViewModel setup)
        {
            _setup = setup ?? throw new ArgumentNullException(nameof(setup));

            Device = setup.Device;
            Host = setup.Host;
            Brightness = setup.Brightness;
            BuildMonitors = new ReadOnlyCollection<IBuildMonitor>(setup.Builds.Select(build =>
                    new BuildMonitor(new BuildStatusViewModel(build), new TeamCityApi(setup.Host, build.Id), build))
                .ToList<IBuildMonitor>());

            // todo make this dynamic instead of a special case
            if (BuildMonitors.Count == 2)
            {
                // assume 8 LEDs
                setup.Builds[0].AllLedIndexes = new[] {0, 1, 2, 3};
                setup.Builds[0].FirstRunningQueuedLedIndex = 3;
                setup.Builds[0].SecondRunningQueuedledIndex = 2;
                setup.Builds[1].AllLedIndexes = new[] {4, 5, 6, 7};
                setup.Builds[1].FirstRunningQueuedLedIndex = 4;
                setup.Builds[1].SecondRunningQueuedledIndex = 5;
            }

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
                LastUpdated = "Updated  " + (DateTime.Now - _lastUpdatedTime.Value).ToAgeString();
            }
        }

        private async Task ExecuteRefreshAsync()
        {
            if (_leds <= 0)
            {
                _leds = await Device.GetLedCountAsync();
            }
            
            var colors = new Color[_leds];
            var blinkColors = new Color[_leds];
            var blink = false;

            foreach (var buildMonitor in BuildMonitors)
            {
                var summary = await buildMonitor.Api.RefreshAsync();
                var previousStatus = buildMonitor.Status.OverallStatus;

                buildMonitor.Status.Update(summary);

                var newStatus = buildMonitor.Status.OverallStatus;
                var overallStatusColor = newStatus == Status.Stale ? buildMonitor.Setup.Stale
                    : newStatus == Status.Success ? buildMonitor.Setup.Success
                    : buildMonitor.Setup.Failure;
                foreach (var i in buildMonitor.Setup.AllLedIndexes)
                {
                    colors[i] = overallStatusColor;
                    if (previousStatus != newStatus)
                    {
                        blinkColors[i] = overallStatusColor;
                        blink = true;
                    }
                }

                var nextIndex = buildMonitor.Setup.FirstRunningQueuedLedIndex;
                if (buildMonitor.Status.IsQueued)
                {
                    colors[nextIndex] = buildMonitor.Setup.Queued;
                    nextIndex = buildMonitor.Setup.SecondRunningQueuedledIndex;
                }
                if (buildMonitor.Status.IsRunning)
                {
                    colors[nextIndex] = buildMonitor.Setup.Running;
                }
            }

            if (blink)
            {
                Scale(ref blinkColors);
                await Device.BlinkAsync(blinkColors, 5, 200);
            }
            Scale(ref colors);
            await Device.SetColorsAsync(colors);

            _lastUpdatedTime = DateTime.Now;
        }

        private void Scale(ref Color[] colors)
        {
            for (var i = 0; i < colors.Length; i++)
            {
                var hsv = colors[i].ToHsv();
                hsv.V = Brightness / 100; // apply the brightness
                colors[i] = hsv.ToArgb();
            }
        }

        public void Dispose()
        {
            _refreshAgeTimer.Stop();
            _autoRefreshTimer.Stop();
        }
    }
}
