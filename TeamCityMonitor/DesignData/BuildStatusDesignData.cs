using System.ComponentModel;
using System.Runtime.CompilerServices;
using Api.Models;
using MVVM.Annotations;
using TeamCityMonitor.Interfaces;

namespace TeamCityMonitor.DesignData
{
    public class BuildStatusDesignData : IBuildStatus
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; }
        public string Id { get; set; }
        public Status OverallStatus { get; set; }
        public bool IsSuccessful { get; set; }
        public string StatusText { get; set; }
        public string LastChanged { get; set; }
        public string RunningUrl { get; set; }
        public bool IsRunning { get; set; }
        public string OverallUrl { get; set; }
        public string LastUrl { get; set; }
        public bool IsQueued { get; set; }
        public string Investigator { get; set; }
        public bool IsUnderInvestigation { get; set; }
        public bool IsStale { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsApiError { get; set; }
        public void Update(BuildTypeStatusSummary summary) { }
        public void RefreshTimeDependentProperties() { }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
