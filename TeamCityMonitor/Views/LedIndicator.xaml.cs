using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public static readonly DependencyProperty ByteSourceProperty = DependencyProperty.Register(nameof(ByteSource), typeof(ObservableCollection<byte>),
            typeof(LedIndicator), new PropertyMetadata(null, (o, args) => ((LedIndicator)o).ByteSourceChanged(args)));

        public ObservableCollection<byte> ByteSource
        {
            get => (ObservableCollection<byte>) GetValue(ByteSourceProperty);
            set => SetValue(ByteSourceProperty, value);
        }

        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(int),
            typeof(LedIndicator), new PropertyMetadata(0, (o, args) => ((LedIndicator)o).UpdateColor()));

        public int Index
        {
            get => (int)GetValue(IndexProperty);
            set => SetValue(IndexProperty, value);
        }

        private void ByteSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= ByteSourceCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += ByteSourceCollectionChanged;
            }

            UpdateColor();
        }

        private void ByteSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewStartingIndex >= Index && e.NewStartingIndex < Index + 3)
            {
                UpdateColor();
            }
            //UpdateColor();
        }

        private void UpdateColor()
        {
            var bytes = ByteSource;
            var i = Index;
            var color = i + 3 <= bytes?.Count
                ? Color.FromArgb(255, bytes[i], bytes[i + 1], bytes[i + 2])
                : Colors.Black;
            Brush = new SolidColorBrush(color);
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
