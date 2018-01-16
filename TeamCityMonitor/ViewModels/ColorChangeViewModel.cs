using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Interfaces;

namespace TeamCityMonitor.ViewModels
{
    class ColorChangeViewModel : IColorChangeViewModel
    {
        public Color OriginalColor { get; }
        public byte OriginalBrightness { get; }
        public Color NewColor { get; set; }
        public byte NewBrightness { get; set; }
        public bool Accepted { get; set; }

        public ColorChangeViewModel(Color color, byte brightness)
        {
            OriginalColor = color;
            OriginalBrightness = brightness;
        }
    }
}
