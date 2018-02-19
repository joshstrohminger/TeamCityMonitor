using BlinkStickUniversal;

namespace Interfaces
{
    public interface ILabeledColor
    {
        string Name { get; }
        RgbColor Color { get; set; }
    }
}