using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BlinkStickUniversal;
using Interfaces;
using MVVM;
using MVVM.Annotations;

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
        public IRelayCommand Monitor { get; } = new RelayCommand(() => { });
        public IBlinkStick Device { get; set; }

        public SetupViewModelDesignData()
        {
            ActiveBuild = Builds[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
