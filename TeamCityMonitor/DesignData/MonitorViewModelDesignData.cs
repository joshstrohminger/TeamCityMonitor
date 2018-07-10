using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BlinkStickUniversal;
using DesignData;
using MVVM;
using MVVM.Annotations;
using TeamCityMonitor.Interfaces;
using TeamCityMonitor.Models;

namespace TeamCityMonitor.DesignData
{
    public class MonitorViewModelDesignData : IMonitorViewModel
    {
        public void Dispose() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public IBlinkStick Device { get; set; } = new BlinkStickSimulator();
        public string Host { get; set; } = "1.2.3.4";
        public IReadOnlyCollection<IBuildMonitor> BuildMonitors { get; set; }
        public IRelayCommand Refresh { get; set; } = new RelayCommand(() => { });
        public bool AutoRefresh { get; set; } = true;
        public double Brightness { get; set; } = 80;
        public string LastUpdated { get; } = "never";
        public DateTime? Time { get; } = DateTime.Now;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MonitorViewModelDesignData()
        {
            BuildMonitors = new IBuildMonitor[]
            {
                new BuildMonitor(new BuildStatusDesignData(), new TeamCityApiDesignData(), new BuildViewModelDesignData{Name = "Build 1", Id = "ID 1"}),
                new BuildMonitor(new BuildStatusDesignData(), new TeamCityApiDesignData(), new BuildViewModelDesignData{Name = "Build 2", Id = "ID 2"}),
            };
        }
    }
}
