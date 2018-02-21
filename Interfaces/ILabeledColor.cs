using Windows.UI;

namespace Interfaces
{
    public interface ILabeledColor
    {
        string Name { get; }
        Color Color { get; set; }
    }
}