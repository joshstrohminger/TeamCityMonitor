using BlinkStickUniversal;
using Interfaces;

namespace DesignData
{
    public class BuildViewModelDesignData : IBuildViewModel
    {
        public RgbColor Investigating => Colors[4].Color;
        public RgbColor Queued => Colors[2].Color;
        public RgbColor Running => Colors[3].Color;
        public RgbColor Failure => Colors[1].Color;
        public RgbColor Idle => Colors[5].Color;
        public RgbColor Success => Colors[0].Color;

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
