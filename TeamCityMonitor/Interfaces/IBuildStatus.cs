using System.ComponentModel;
using Api.Models;

namespace TeamCityMonitor.Interfaces
{
    public interface IBuildStatus : INotifyPropertyChanged
    {
        string Name { get; }
        string Id { get; }
        bool IsSuccessful { get; }
        string StatusText { get; }
        string LastChanged { get; }
        string RunningUrl { get; }
        bool IsRunning { get; }
        string OverallUrl { get;}
        string LastUrl { get; }
        bool IsQueued { get; }
        string Investigator { get; }
        bool IsUnderInvestigation { get; }
        bool IsStale { get; }
        string ErrorMessage { get; }
        bool IsApiError { get; }
        void Update(BuildTypeStatusSummary summary);
    }
}