#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick application.
//
// BlinkStick application is free software: you can redistribute 
// it and/or modify it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//		
// BlinkStick application is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick application. If not, see http://www.gnu.org/licenses/.
#endregion

using System;

namespace BlinkStickUniversal
{
	public struct HslColor 
	{
	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = Lightness.GetHashCode();
	            hashCode = (hashCode * 397) ^ Hue.GetHashCode();
	            hashCode = (hashCode * 397) ^ Saturation.GetHashCode();
	            return hashCode;
	        }
	    }
        
		// http://en.wikipedia.org/wiki/HSL_color_space
        public double Lightness { get; }

		public double Hue { get; }

	    public double Saturation { get; }

		public HslColor(double hue, double saturation, double lightness)
		{
			Hue = hue % 360;
		    if (Hue < 0)
		    {
		        Hue += 360;
		    }
			Saturation = Math.Max(0, Math.Min(1, saturation));
			Lightness = Math.Max(0, Math.Min(1, lightness));
		}

		public static HslColor FromRgbColor(RgbColor cc)
		{
			var r = cc.R / 255d;
			var g = cc.G / 255d;
			var b = cc.B / 255d;
			
			var min = Math.Min(Math.Min(r, g), b);
			var max = Math.Max(Math.Max(r, g), b);
			// calulate hue according formula given in
			// "Conversion from RGB to HSL or HSV"
			double hue = 0;
		    double saturation = 1;

			if (min != max)
			{
				if (r == max && g >= b)
				{
					hue = 60 * ((g - b) / (max - min)) + 0;
				}
				else
				if (r == max && g < b)
				{
					hue = 60 * ((g - b) / (max - min)) + 360;
				}
				else
				if (g == max)
				{
					hue = 60 * ((b - r) / (max - min)) + 120;
				}
				else
				if (b == max)
				{
					hue = 60 * ((r - g) / (max - min)) + 240;
				}
			}
			// find lightness
			var lightness = (min+max)/2;

			// find saturation
			if (lightness == 0 || min == max)
				saturation = 0;
			else
			if (lightness > 0 && lightness <= 0.5)
				saturation = (max-min)/(2*lightness);
			else
			if (lightness > 0.5)
				saturation = (max-min)/(2-2*lightness);

            return new HslColor(hue, saturation, lightness);
		}

	    public RgbColor ToRgbColor()
		{
			// convert to RGB according to
			// "Conversion from HSL to RGB"

			var r = Lightness;
			var g = Lightness;
			var b = Lightness;
			if (Saturation == 0)
				return new RgbColor((int)(r*255), (int)(g*255), (int)(b*255));

			double q;
			if (Lightness < 0.5)
				q = Lightness * (1 + Saturation);
			else
				q = Lightness + Saturation - (Lightness * Saturation);
			var p = 2 * Lightness - q;
			var hk = Hue / 360;

			// r,g,b colors
			double[] tc = { hk + (1d/3d), hk, hk-(1d/3d)};
			double[] colors = {0, 0, 0};

			for (int color = 0; color < colors.Length; color++)
			{
				if (tc[color] < 0)
					tc[color] += 1;
				if (tc[color] > 1)
					tc[color] -= 1;

				if (tc[color] < (1d/6d))
					colors[color] = p + ((q-p)*6*tc[color]);
				else
				if (tc[color] >= (1d/6d) && tc[color] < (1d/2d))
					colors[color] = q;
				else
				if (tc[color] >= (1d/2d) && tc[color] < (2d/3d))
					colors[color] = p + ((q-p)*6*(2d/3d - tc[color]));
				else
					colors[color] = p;

				colors[color] *= 255; // convert to value expected by Color
			}
			return new RgbColor((int)colors[0], (int)colors[1], (int)colors[2]);
		}

		public static bool operator != (HslColor left, HslColor right)
		{
			return !left.Equals(right);
		}

		public static bool operator == (HslColor left, HslColor right)
		{
		    return left.Equals(right);
		}
	    public bool Equals(HslColor other)
	    {
	        return Lightness.Equals(other.Lightness) && Hue.Equals(other.Hue) && Saturation.Equals(other.Saturation);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        return obj is HslColor color && Equals(color);
	    }
        public override string ToString() => $"HSL({Hue:f2}, {Saturation:f2}, {Lightness:f2}) RGB({ToRgbColor()})"; 
	}
}
