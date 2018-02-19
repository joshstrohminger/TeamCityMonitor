using System.Collections.ObjectModel;
using BlinkStickUniversal;
using Interfaces;
using MVVM;

namespace DesignData
{
    public class SetupViewModelDesignData : ISetupViewModel
    {
        public string Host { get; set; } = "192.168.123.456";
        public double Brightness { get; set; } = 83;

        public ObservableCollection<IBuildViewModel> Builds { get; } = new ObservableCollection<IBuildViewModel>(new []
        {
            new BuildViewModelDesignData(),
            new BuildViewModelDesignData(),
            new BuildViewModelDesignData()
        });

        public IBuildViewModel ActiveBuild { get; set; }
        public IRelayCommand AddBuild { get; } = new RelayCommand(() => { });
        public IRelayCommand RemoveBuild { get; } = new RelayCommand(() => { });
        public IRelayCommand ApplyToAllBuilds { get; } = new RelayCommand(() => { });

        public SetupViewModelDesignData()
        {
            ActiveBuild = Builds[0];
        }
    }
}
