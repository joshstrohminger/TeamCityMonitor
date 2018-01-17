using System;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Interfaces;
using Microsoft.Toolkit.Uwp.Helpers;
using ColorHelper = Microsoft.Toolkit.Uwp.Helpers.ColorHelper;

namespace TeamCityMonitor.ViewModels
{
    class ColorChangeViewModel : IColorChangeViewModel
    {
        public Color OriginalColor { get; }
        public byte OriginalBrightness { get; }
        public Color WorkingColor { get; }
        public Color NewColor { get; private set; }
        public byte NewBrightness { get; private set; }
        public bool Accepted { get; private set; }
        public Rectangle Source { get; set; }

        public ColorChangeViewModel(Color color, byte brightness)
        {
            OriginalColor = color;
            OriginalBrightness = brightness;
            var hsv = color.ToHsv();
            WorkingColor = ColorHelper.FromHsv(hsv.H, hsv.S, brightness / 100d);
        }

        public void ChangeColor(Color color)
        {
            var hsv = color.ToHsv();
            NewBrightness = (byte)Math.Round(hsv.V * 100, MidpointRounding.AwayFromZero);
            NewColor = ColorHelper.FromHsv(hsv.H, hsv.S, 1);
            Accepted = true;
        }
    }
}
