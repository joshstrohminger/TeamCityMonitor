using System.ComponentModel;
using Windows.UI;

namespace Interfaces
{
    public interface IBuildViewModel : INotifyPropertyChanged
    {
        Color Investigating { get; }
        Color Queued { get; }
        Color Running { get; }
        Color Failure { get; }
        Color Idle { get; }
        Color Success { get; }
        Color Stale { get; }
        Color ApiError { get; }
        Color ApiCall { get; }
        ILabeledColor[] Colors { get; }
        string Id { get; set; }
        string Name { get; set; }
        int[] AllLedIndexes { get; set; }
        int FirstRunningQueuedLedIndex { get; set; }
        int SecondRunningQueuedledIndex { get; set; }
    }
}