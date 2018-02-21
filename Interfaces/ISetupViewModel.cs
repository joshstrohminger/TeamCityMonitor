using System.Collections.ObjectModel;
using MVVM;

namespace Interfaces
{
    public interface ISetupViewModel
    {
        string Host { get; set; }
        ObservableCollection<IBuildViewModel> Builds { get; }
        double Brightness { get; set; }
        IBuildViewModel ActiveBuild { get; set; }
        IRelayCommand AddBuild { get; }
        IRelayCommand RemoveBuild { get; }
        IRelayCommand ApplyToAllBuilds { get; }
        IRelayCommand Monitor { get; }
    }
}