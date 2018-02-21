using Windows.UI;
using BlinkStickUniversal;
using Interfaces;

namespace DesignData
{
    public class BuildViewModelDesignData : IBuildViewModel
    {
        public Color Investigating => Colors[4].Color;
        public Color Queued => Colors[2].Color;
        public Color Running => Colors[3].Color;
        public Color Failure => Colors[1].Color;
        public Color Idle => Colors[5].Color;
        public Color Success => Colors[0].Color;

        public ILabeledColor[] Colors { get; } = {
            new LabeledColorDesignData("Success", "green"),
            new LabeledColorDesignData("Failure", "red"),
            new LabeledColorDesignData("Queued", "yellow"),
            new LabeledColorDesignData("Running", "blue"),
            new LabeledColorDesignData("Investigating", "orange"),
            new LabeledColorDesignData("Idle", "black")
        };

        public string Id { get; set; } = "buildidgoeshere";
        public string Name { get; set; } = "New build";
    }
}
