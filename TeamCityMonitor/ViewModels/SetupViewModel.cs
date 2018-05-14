using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BlinkStickUniversal;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.ViewModels
{
    public class SetupViewModel : ObservableObject, ISetupViewModel
    {
        private readonly ILinearNavigator _linearNavigator;
        private string _host;
        private double _brightness = 100;
        private IBuildViewModel _activeBuild;

        public IRelayCommand AddBuild { get; }
        public IRelayCommand RemoveBuild { get; }
        public IRelayCommand ApplyToAllBuilds { get; }
        public IRelayCommand Monitor { get; }
        public IBlinkStick Device { get; set; }

        public IBuildViewModel ActiveBuild
        {
            get => _activeBuild;
            set
            {
                _activeBuild = value;
                OnPropertyChanged();
                RemoveBuild.RaiseCanExecuteChanged();
                ApplyToAllBuilds.RaiseCanExecuteChanged();
                Monitor.RaiseCanExecuteChanged();
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
                Monitor.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<IBuildViewModel> Builds { get; } = new ObservableCollection<IBuildViewModel>();

        public SetupViewModel(ILinearNavigator linearNavigator)
        {
            _linearNavigator = linearNavigator ?? throw new ArgumentNullException(nameof(linearNavigator));
            AddBuild = new RelayCommand(ExecuteAddBuild);
            RemoveBuild = new RelayCommand(ExecuteRemoveBuild, CanExecuteRemoveBuild);
            ApplyToAllBuilds = new RelayCommand(ExecuteApplyToAllBuilds, CanExecuteApplyToAllBuilds);
            Monitor = new RelayCommand(ExecuteMonitor, CanExecuteMonitor);
        }

        private bool CanExecuteMonitor() => Builds.Count >= 1 && !string.IsNullOrWhiteSpace(Host) &&
                                            Builds.All(build => !string.IsNullOrWhiteSpace(build.Id));

        private void ExecuteMonitor()
        {
            if (!CanExecuteMonitor()) return;
            _linearNavigator.GoForward();
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
            ActiveBuild.PropertyChanged -= BuildOnPropertyChanged;
            ActiveBuild = null;
        }

        private void ExecuteAddBuild()
        {
            var build = new BuildViewModel();
            Builds.Add(build);
            ActiveBuild = build;
            ActiveBuild.PropertyChanged += BuildOnPropertyChanged;
        }

        private void BuildOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(IBuildViewModel.Id))
            {
                Monitor.RaiseCanExecuteChanged();
            }
        }
    }
}
