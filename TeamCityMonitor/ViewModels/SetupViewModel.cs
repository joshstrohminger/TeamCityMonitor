using System.Collections.ObjectModel;
using System.Linq;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.ViewModels
{
    public class SetupViewModel : ObservableObject, ISetupViewModel
    {
        private string _host;
        private double _brightness;
        private IBuildViewModel _activeBuild;

        public IRelayCommand AddBuild { get; }
        public IRelayCommand RemoveBuild { get; }
        public IRelayCommand ApplyToAllBuilds { get; }

        public IBuildViewModel ActiveBuild
        {
            get => _activeBuild;
            set
            {
                _activeBuild = value;
                OnPropertyChanged();
                RemoveBuild.RaiseCanExecuteChanged();
                ApplyToAllBuilds.RaiseCanExecuteChanged();
            }
        }

        public double Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                OnPropertyChanged();
            }
        }

        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IBuildViewModel> Builds { get; } = new ObservableCollection<IBuildViewModel>();

        public SetupViewModel()
        {
            AddBuild = new RelayCommand(ExecuteAddBuild);
            RemoveBuild = new RelayCommand(ExecuteRemoveBuild, CanExecuteRemoveBuild);
            ApplyToAllBuilds = new RelayCommand(ExecuteApplyToAllBuilds, CanExecuteApplyToAllBuilds);
        }

        private bool CanExecuteApplyToAllBuilds() => Builds.Count >= 2;

        private void ExecuteApplyToAllBuilds()
        {
            if (!CanExecuteApplyToAllBuilds()) return;

            foreach (var build in Builds.Except(new []{ActiveBuild}))
            {
                for (var i = 0; i < build.Colors.Length; i++)
                {
                    build.Colors[i].Color = ActiveBuild.Colors[i].Color;
                }
            }
        }

        private bool CanExecuteRemoveBuild() => ActiveBuild != null;

        private void ExecuteRemoveBuild()
        {
            if (!CanExecuteRemoveBuild()) return;

            Builds.Remove(ActiveBuild);
            ActiveBuild = null;
        }

        private void ExecuteAddBuild()
        {
            var build = new BuildViewModel();
            Builds.Add(build);
            ActiveBuild = build;
        }
    }
}
