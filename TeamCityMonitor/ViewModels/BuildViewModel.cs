using Windows.UI;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.ViewModels
{
    public class BuildViewModel : ObservableObject, IBuildViewModel
    {
        private string _id;
        private string _name = "new build";

        public ILabeledColor[] Colors { get; } =
        {
            new LabeledColor("Success", "#00FF00"),
            new LabeledColor("Failure", "#FF0000"),
            new LabeledColor("Queued", "#FFFF00"),
            new LabeledColor("Running", "#00FFFF"),
            new LabeledColor("Investigating", "#FFA500"),
            new LabeledColor("Stale", "#FF00FF"),
            new LabeledColor("Idle", "#000000")
        };

        public Color Success => Colors[0].Color;
        public Color Failure => Colors[1].Color;
        public Color Queued => Colors[2].Color;
        public Color Running => Colors[3].Color;
        public Color Investigating => Colors[4].Color;
        public Color Stale => Colors[5].Color;
        public Color Idle => Colors[6].Color;
        public int[] AllLedIndexes { get; set; }
        public int FirstRunningQueuedLedIndex { get; set; }
        public int SecondRunningQueuedledIndex { get; set; }

        public string Id
        {
            get => _id;
            set => UpdateOnPropertyChanged(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => UpdateOnPropertyChanged(ref _name, value);
        }
    }
}
