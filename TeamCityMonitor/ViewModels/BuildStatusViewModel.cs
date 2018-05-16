using System;
using System.ComponentModel;
using System.Linq;
using Interfaces;
using MVVM;
using TeamCityMonitor.Models;
using Windows.UI.Xaml;

namespace ViewModels
{
    public class BuildStatusViewModel : ObservableObject, IBuildStatus
    {
        #region Fields

        private readonly TimeSpan _staleCriteria = TimeSpan.FromHours(5);
        private readonly DispatcherTimer _timer;

        private DateTime? _timeLastChanged;
            
        #endregion

        #region Backing Fields
        
        private bool _isSuccessful;
        private string _statusText;
        private string _lastChanged;
        private string _runningUrl;
        private bool _isRunning;
        private string _url;
        private bool _isQueued;
        private string _investigator;
        private bool _isUnderInvestigation;
        private bool _isStale;
        private string _errorMessage;

        #endregion

        #region Properties

        public string Name {get;}
        public string Id {get;}

        public bool IsSuccessful
        {
            get => _isSuccessful;
            private set => UpdateOnPropertyChanged(ref _isSuccessful, value);
        }

        public string StatusText
        {
            get => _statusText;
            private set => UpdateOnPropertyChanged(ref _statusText, value);
        }

        public string LastChanged
        {
            get => _lastChanged;
            private set => UpdateOnPropertyChanged(ref _lastChanged, value);
        }

        public string RunningUrl
        {
            get => _runningUrl;
            private set => UpdateOnPropertyChanged(ref _runningUrl, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            private set => UpdateOnPropertyChanged(ref _isRunning, value);
        }

        public string Url
        {
            get => _url;
            private set => UpdateOnPropertyChanged(ref _url, value);
        }

        public bool IsQueued
        {
            get => _isQueued;
            private set => UpdateOnPropertyChanged(ref _isQueued, value);
        }

        public string Investigator
        {
            get => _investigator;
            private set => UpdateOnPropertyChanged(ref _investigator, value);
        }

        public bool IsUnderInvestigation
        {
            get => _isUnderInvestigation;
            private set => UpdateOnPropertyChanged(ref _isUnderInvestigation, value);
        }

        public bool IsStale
        {
            get => _isStale;
            private set => UpdateOnPropertyChanged(ref _isStale, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => UpdateOnPropertyChanged(ref _errorMessage, value);
        }
        
        #endregion
    
        #region Construction

        public BuildStatusViewModel(IBuildViewModel setup)
        {
            if(setup is null) throw new ArgumentNullException(nameof(setup));
            Name = setup.Name;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (o,e) => RefreshTimeDependentProperties();
            _timer.Start();
            // todo need to stop the timer somehow, make this disposable maybe?
        }
            
        #endregion

        #region Members

        public void Update(BuildTypeStatusSummary summary)
        {
            if(summary is null) throw new ArgumentNullException(nameof(summary));

            ErrorMessage = summary.ErrorMessage;
            if(summary.IsSuccessful)
            {
                Investigator = summary.Investigations?.Investigations?.FirstOrDefault()?.Assignee?.Name;
                IsUnderInvestigation = !string.IsNullOrWhiteSpace(Investigator);

                var lastFinishedBuild = summary.Builds.Builds.FirstOrDefault(build => build.State == "finished");
                _timeLastChanged = ParseDateTime(lastFinishedBuild?.FinishDate);
                IsSuccessful = lastFinishedBuild?.Status == "SUCCESS";
                StatusText = lastFinishedBuild?.StatusText;
                Url = lastFinishedBuild?.WebUrl ?? summary.WebUrl;

                IsQueued = summary.Builds.Builds.Any(build => build.State == "queued");

                RunningUrl = summary.Builds.Builds.FirstOrDefault(build => build.State == "running")?.WebUrl;
                IsRunning = !string.IsNullOrWhiteSpace(RunningUrl);
            }

            // todo Decide on how to set the LEDs and whether any flashing is needed for state changes, or handle it ouside of this class

            RefreshTimeDependentProperties();
        }

        private DateTime? ParseDateTime(string dateTimeString)
        {
            return DateTime.Now; // todo actually parse this
        }

        private void RefreshTimeDependentProperties()
        {
            if(_timeLastChanged.HasValue)
            {
                var age = DateTime.Now - _timeLastChanged.Value;
                IsStale = age > _staleCriteria;
                LastChanged = age.ToString();
                // todo use library to come up with a nicer string
            }
            else
            {
                IsStale = true;
                LastChanged = "NEVER";
            }
        }
            
        #endregion
    }
}