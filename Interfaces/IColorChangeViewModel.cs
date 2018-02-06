using Windows.UI;

namespace Interfaces
{
    public interface IColorChangeViewModel
    {
        Color OriginalColor { get; }
        byte OriginalBrightness { get; }
        Color WorkingColor { get; }
        Color NewColor { get; }
        Color ResultingColor { get; }
        byte NewBrightness { get; }
        bool Accepted { get; }
        void ChangeColor(Color color);
    }
}
