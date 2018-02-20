using System;
using Windows.UI;
using Microsoft.Toolkit.Uwp;

namespace BlinkStickUniversal
{
    public static class ColorExtensions
    {
        public static Color ToArgb(this HsvColor hsv)
        {
            var hi = Convert.ToInt32(Math.Floor(hsv.H / 60)) % 6;
            var f = hsv.H / 60 - Math.Floor(hsv.H / 60);

            hsv.V = hsv.V * 255;
            var v = Convert.ToByte(hsv.V);
            var p = Convert.ToByte(hsv.V * (1 - hsv.S));
            var q = Convert.ToByte(hsv.V * (1 - f * hsv.S));
            var t = Convert.ToByte(hsv.V * (1 - (1 - f) * hsv.S));

            switch (hi)
            {
                case 0:
                    return Color.FromArgb(255, v, t, p);
                case 1:
                    return Color.FromArgb(255, q, v, p);
                case 2:
                    return Color.FromArgb(255, p, v, t);
                case 3:
                    return Color.FromArgb(255, p, q, v);
                case 4:
                    return Color.FromArgb(255, t, p, v);
                default:
                    return Color.FromArgb(255, v, p, q);
            }
        }
    }
}
