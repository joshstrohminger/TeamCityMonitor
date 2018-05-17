using System;
using System.Collections.Generic;
using System.ComponentModel;
using BlinkStickUniversal;
using MVVM;

namespace TeamCityMonitor.Interfaces
{
    public interface IMonitorViewModel : IDisposable, INotifyPropertyChanged
    {
        IBlinkStick Device { get; }
        string Host { get; }
        IReadOnlyCollection<IBuildMonitor> BuildMonitors { get; }
        IRelayCommand Refresh { get; }
        bool AutoRefresh { get; set; }
        double Brightness { get; set; }
    }
}
