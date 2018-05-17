using System.Collections.ObjectModel;
using System.ComponentModel;
using BlinkStickUniversal;
using MVVM;

namespace Interfaces
{
    public interface ISetupViewModel : INotifyPropertyChanged
    {
        string Host { get; set; }
        ObservableCollection<IBuildViewModel> Builds { get; }
        double Brightness { get; set; }
        IBuildViewModel ActiveBuild { get; set; }
        IRelayCommand AddBuild { get; }
        IRelayCommand RemoveBuild { get; }
        IRelayCommand ApplyToAllBuilds { get; }
        IRelayCommand Monitor { get; }
        IBlinkStick Device { get; set; }
    }
}