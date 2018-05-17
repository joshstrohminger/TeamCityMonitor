using System;
using System.Globalization;
using System.Linq;
using Windows.UI.Xaml;
using Api.Models;
using Interfaces;
using MVVM;
using TeamCityMonitor.Interfaces;

namespace TeamCityMonitor.ViewModels
{
    public class BuildStatusViewModel : ObservableObject, IBuildStatus, IDisposable
    {
        #region Fields

        private readonly TimeSpan _staleCriteria = TimeSpan.FromHours(5);
        private readonly DispatcherTimer _timer;
            
        #endregion

        #region Backing Fields
        
        private bool _isSuccessful;
        private string _statusText;
        private string _lastChanged;
        private string _runningUrl;
        private bool _isRunning;
        private string _lastUrl;
        private string _overallUrl;
        private bool _isQueued;
        private string _investigator;
        private bool _isUnderInvestigation;
        private bool _isStale;
        private string _errorMessage;
        private DateTime? _timeLastChanged;
        private bool _isApiError;

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

        public string LastUrl
        {
            get => _lastUrl;
            private set => UpdateOnPropertyChanged(ref _lastUrl, value);
        }

        public string OverallUrl
        {
            get => _overallUrl;
            private set => UpdateOnPropertyChanged(ref _overallUrl, value);
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

        public bool IsApiError
        {
            get => _isApiError;
            private set => UpdateOnPropertyChanged(ref _isApiError, value);
        }

        #endregion
    
        #region Construction

        public BuildStatusViewModel(IBuildViewModel setup)
        {
            if(setup is null) throw new ArgumentNullException(nameof(setup));
            Name = setup.Name;
            Id = setup.Id;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timer.Tick += (sender, o) => RefreshTimeDependentProperties();
            _timer.Start();
        }
            
        #endregion

        #region Members

        public void Update(BuildTypeStatusSummary summary)
        {
            if(summary is null) throw new ArgumentNullException(nameof(summary));

            ErrorMessage = summary.ErrorMessage;
            IsApiError = !summary.IsSuccessful;
            if(summary.IsSuccessful)
            {
                Investigator = summary.Investigations?.Investigations?.FirstOrDefault()?.Assignee?.Name;
                IsUnderInvestigation = !string.IsNullOrWhiteSpace(Investigator);

                var lastFinishedBuild = summary.Builds.Builds.FirstOrDefault(build => build.State == "finished");
                _timeLastChanged = ParseDateTime(lastFinishedBuild?.FinishDate);
                IsSuccessful = lastFinishedBuild?.Status == "SUCCESS";
                StatusText = lastFinishedBuild?.StatusText;
                LastUrl = lastFinishedBuild?.WebUrl;
                OverallUrl = summary.WebUrl;

                IsQueued = summary.Builds.Builds.Any(build => build.State == "queued");

                RunningUrl = summary.Builds.Builds.FirstOrDefault(build => build.State == "running")?.WebUrl;
                IsRunning = !string.IsNullOrWhiteSpace(RunningUrl);
            }

            // todo Decide on how to set the LEDs and whether any flashing is needed for state changes, or handle it ouside of this class

            RefreshTimeDependentProperties();
        }

        private DateTime? ParseDateTime(string dateTimeString)
        {
            return dateTimeString is null ? (DateTime?)null : DateTime.ParseExact(dateTimeString, "yyyyMMdd'T'HHmmsszzzz", CultureInfo.InvariantCulture);
        }

        private void RefreshTimeDependentProperties()
        {
            if(_timeLastChanged.HasValue)
            {
                var age = DateTime.Now - _timeLastChanged.Value;
                IsStale = age > _staleCriteria;
                LastChanged = GetAgeString(age);
            }
            else
            {
                IsStale = true;
                LastChanged = "never";
            }
        }

        private string GetAgeString(TimeSpan age)
        {
            var seconds = age.TotalSeconds;
            var minutes = age.TotalMinutes;
            var hours = age.TotalHours;
            var days = age.TotalDays;

            if (seconds < 5) return "now";
            if (seconds < 60) return $"{seconds:0}s ago";
            if (minutes < 60) return $"{minutes:0}m ago";
            if (hours < 24) return $"{hours:0}h ago";
            return $"{days:0} days ago";
        }

        public void Dispose()
        {
            _timer.Stop();
        }

        public override string ToString() => Id;

        #endregion
    }
}