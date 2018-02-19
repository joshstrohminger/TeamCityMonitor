using BlinkStickUniversal;
using Interfaces;

namespace DesignData
{
    public class LabeledColorDesignData : ILabeledColor
    {
        public string Name { get; }
        public RgbColor Color { get; set; }

        public LabeledColorDesignData()
        {
        }

        public LabeledColorDesignData(string name, string color)
        {
            Name = name;
            Color = RgbColor.FromString(color);
        }
    }
}
