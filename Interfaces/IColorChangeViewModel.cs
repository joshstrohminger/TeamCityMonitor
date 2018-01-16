using Windows.UI;

namespace Interfaces
{
    public interface IColorChangeViewModel
    {
        Color OriginalColor { get; }
        byte OriginalBrightness { get; }
        Color NewColor { get; set; }
        byte NewBrightness { get; set; }
        bool Accepted { get; set; }
    }
}
