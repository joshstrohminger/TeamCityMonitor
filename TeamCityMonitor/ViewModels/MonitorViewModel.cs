using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Api;
using BlinkStickUniversal;
using Interfaces;
using MVVM;
using TeamCityMonitor.Interfaces;
using TeamCityMonitor.Models;

namespace TeamCityMonitor.ViewModels
{
    public class MonitorViewModel : ObservableObject, IMonitorViewModel
    {
        #region Fields

        private bool _autoRefresh = true;
        private double _brightness;
        private readonly DispatcherTimer _timer;

        #endregion

        #region Properties

        public IBlinkStick Device { get; }
        public string Host { get; }
        public IReadOnlyCollection<IBuildMonitor> BuildMonitors { get; }
        public IRelayCommand Refresh { get; }

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
                        _timer.Start();
                    }
                    else
                    {
                        _timer.Stop();
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
            if(setup is null) throw new ArgumentNullException(nameof(setup));

            Device = setup.Device;
            Host = setup.Host;
            BuildMonitors = new ReadOnlyCollection<IBuildMonitor>(setup.Builds.Select(build =>
                    new BuildMonitor(new BuildStatusViewModel(build), new TeamCityApi(setup.Host, build.Id), build))
                .ToList<IBuildMonitor>());
            Refresh = new RelayCommand(async () => await ExecuteRefreshAsync());
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMinutes(1)};
            _timer.Tick += async (sender, o) => await ExecuteRefreshAsync();
            _timer.Start();
            //ExecuteRefreshAsync();
        }

        private async Task ExecuteRefreshAsync()
        {
            foreach (var buildMonitor in BuildMonitors)
            {
                var summary = await buildMonitor.Api.RefreshAsync();
                buildMonitor.Status.Update(summary);
                // todo should colors be updated here?
            }
        }

        public void Dispose()
        {
            _timer.Stop();
        }
    }
}
