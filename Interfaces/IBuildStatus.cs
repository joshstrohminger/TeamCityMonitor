using System;
using System.ComponentModel;
using Windows.UI;

namespace Interfaces
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
        string Url { get;}
        bool IsQueued { get; }
        string Investigator { get; }
        bool IsUnderInvestigation { get; }
        bool IsStale { get; }
        string ErrorMessage { get; }
    }
}