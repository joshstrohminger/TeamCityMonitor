using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Interfaces;
using MVVM.Annotations;

namespace DesignData
{
    public class BuildViewModelDesignData : IBuildViewModel
    {
        public Color Investigating => Colors[4].Color;
        public Color Queued => Colors[2].Color;
        public Color Running => Colors[3].Color;
        public Color Failure => Colors[1].Color;
        public Color Stale => Colors[5].Color;
        public Color Idle => Colors[6].Color;
        public Color Success => Colors[0].Color;
        public Color ApiError => Colors[7].Color;
        public int[] AllLedIndexes { get; set; }
        public int FirstRunningQueuedLedIndex { get; set; }
        public int SecondRunningQueuedledIndex { get; set; }

        public ILabeledColor[] Colors { get; } = {
            new LabeledColorDesignData("Success", "#00FF00"),
            new LabeledColorDesignData("Failure", "#FF0000"),
            new LabeledColorDesignData("Queued", "#FFFF00"),
            new LabeledColorDesignData("Running", "#00FFFF"),
            new LabeledColorDesignData("Investigating", "#FFA500"),
            new LabeledColorDesignData("Stale", "#FF00FF"),
            new LabeledColorDesignData("Idle", "#000000"),
            new LabeledColorDesignData("API Error", "#FFFFFF")
        };

        public string Id { get; set; } = "buildidgoeshere";
        public string Name { get; set; } = "New build";
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
