using BlinkStickUniversal;
using DesignData;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.ViewModels
{
    public class BuildViewModel : ObservableObject, IBuildViewModel
    {
        private string _id;
        private string _name = "new build";

        public ILabeledColor[] Colors { get; } = new[]
        {
            new LabeledColor("Success", "green"),
            new LabeledColor("Failure", "red"),
            new LabeledColor("Queued", "yellow"),
            new LabeledColor("Running", "blue"),
            new LabeledColor("Investigating", "orange"),
            new LabeledColor("Idle", "black")
        };

        public RgbColor Success => Colors[0].Color;
        public RgbColor Failure => Colors[1].Color;
        public RgbColor Queued => Colors[2].Color;
        public RgbColor Running => Colors[3].Color;
        public RgbColor Investigating => Colors[4].Color;
        public RgbColor Idle => Colors[5].Color;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }
}
