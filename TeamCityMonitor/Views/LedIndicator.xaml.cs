using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using BlinkStickUniversal;
using Microsoft.Toolkit.Uwp.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TeamCityMonitor.Views
{
    public sealed partial class LedIndicator : UserControl
    {
        private static readonly DependencyProperty BrushProperty = DependencyProperty.Register(nameof(Brush),
            typeof(SolidColorBrush), typeof(LedIndicator), new PropertyMetadata(null));

        private SolidColorBrush Brush
        {
            get => (SolidColorBrush) GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof(R), typeof(byte),
            typeof(LedIndicator), new PropertyMetadata((byte)0, ColorOrBrightnessChanged));

        public byte R
        {
            get => (byte) GetValue(RProperty);
            set => SetValue(RProperty, value);
        }

        public static readonly DependencyProperty GProperty = DependencyProperty.Register(nameof(G), typeof(byte),
            typeof(LedIndicator), new PropertyMetadata((byte)0, ColorOrBrightnessChanged));

        public byte G
        {
            get => (byte)GetValue(GProperty);
            set => SetValue(GProperty, value);
        }

        public static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(byte),
            typeof(LedIndicator), new PropertyMetadata((byte)0, ColorOrBrightnessChanged));

        public byte B
        {
            get => (byte)GetValue(BProperty);
            set => SetValue(BProperty, value);
        }

        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register(nameof(Brightness),
            typeof(double), typeof(LedIndicator), new PropertyMetadata(100d, ColorOrBrightnessChanged));

        public double Brightness
        {
            get => (double) GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }

        private static void ColorOrBrightnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LedIndicator)d).UpdateColor();
        }

        private void UpdateColor()
        {
            var hsv = Color.FromArgb(255, R, G, B).ToHsv();
            hsv.V = Brightness / 100; // apply the brightness
            if (Brush == null)
            {
                Brush = new SolidColorBrush(hsv.ToArgb());
            }
            else
            {
                Brush.Color = hsv.ToArgb();
            }
        }

        public LedIndicator()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            UpdateColor();
        }
    }
}
