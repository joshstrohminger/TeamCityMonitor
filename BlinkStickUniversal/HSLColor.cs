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
using System.Collections.Generic;
using System.Text;

namespace BlinkStickUniversal
{
	public struct HSLColor 
	{
		double m_hue;
		double m_saturation;
		double m_lightness;
		// http://en.wikipedia.org/wiki/HSL_color_space

		public double Hue
		{
			get { return m_hue; }
			set { m_hue = value; }
		}
		public double Saturation
		{
			get { return m_saturation; }
			set { m_saturation = value; }
		}
		public double Lightness
		{
			get { return m_lightness; }
			set
			{ 
				m_lightness = value;
				if (m_lightness < 0)
					m_lightness = 0;
				if (m_lightness > 1)
					m_lightness = 1;
			}
		}
		public HSLColor(double hue, double saturation, double lightness)
		{
			m_hue = Math.Min(360, hue);
			m_saturation = Math.Min(1, saturation);
			m_lightness = Math.Min(1, lightness);
		}
		public HSLColor(RgbColor color)
		{
			m_hue = 0;
			m_saturation = 1;
			m_lightness = 1;
			FromRGB(color);
		}
		public RgbColor Color
		{
			get { return ToRGB(); }
			set { FromRGB(value); }
		}
		void FromRGB(RgbColor cc)
		{
			double r = (double)cc.R / 255d;
			double g = (double)cc.G / 255d;
			double b = (double)cc.B / 255d;
			
			double min = Math.Min(Math.Min(r, g), b);
			double max = Math.Max(Math.Max(r, g), b);
			// calulate hue according formula given in
			// "Conversion from RGB to HSL or HSV"
			m_hue = 0;
			if (min != max)
			{
				if (r == max && g >= b)
				{
					m_hue = 60 * ((g - b) / (max - min)) + 0;
				}
				else
				if (r == max && g < b)
				{
					m_hue = 60 * ((g - b) / (max - min)) + 360;
				}
				else
				if (g == max)
				{
					m_hue = 60 * ((b - r) / (max - min)) + 120;
				}
				else
				if (b == max)
				{
					m_hue = 60 * ((r - g) / (max - min)) + 240;
				}
			}
			// find lightness
			m_lightness = (min+max)/2;

			// find saturation
			if (m_lightness == 0 ||min == max)
				m_saturation = 0;
			else
			if (m_lightness > 0 && m_lightness <= 0.5)
				m_saturation = (max-min)/(2*m_lightness);
			else
			if (m_lightness > 0.5)
				m_saturation = (max-min)/(2-2*m_lightness);
		}
		RgbColor ToRGB()
		{
			// convert to RGB according to
			// "Conversion from HSL to RGB"

			double r = m_lightness;
			double g = m_lightness;
			double b = m_lightness;
			if (m_saturation == 0)
				return RgbColor.FromRgb((int)(r*255), (int)(g*255), (int)(b*255));

			double q = 0;
			if (m_lightness < 0.5)
				q = m_lightness * (1 + m_saturation);
			else
				q = m_lightness + m_saturation - (m_lightness * m_saturation);
			double p = 2 * m_lightness - q;
			double hk = m_hue / 360;

			// r,g,b colors
			double[] tc = new double[3] { hk + (1d/3d), hk, hk-(1d/3d)};
			double[] colors = new double[3] {0, 0, 0};

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
			return RgbColor.FromRgb((int)colors[0], (int)colors[1], (int)colors[2]);
		}

		public static bool operator != (HSLColor left, HSLColor right)
		{
			return !(left == right);
		}
		public static bool operator == (HSLColor left, HSLColor right)
		{
			return (left.Hue == right.Hue && 
					left.Lightness == right.Lightness && 
					left.Saturation == right.Saturation);
		}
		public override string ToString()
		{
			string s = string.Format("HSL({0:f2}, {1:f2}, {2:f2})", Hue, Saturation, Lightness);
			s += string.Format ("RGB({0:x2}{1:x2}{2:x2})", Color.R, Color.G, Color.B); 
			return s;
		}

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
	}
}
