using Windows.UI;
using BlinkStickUniversal;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.ViewModels
{
    public class LabeledColor : ObservableObject, ILabeledColor
    {
        private Color _color;

        public string Name { get; }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        public LabeledColor(string name, string color)
        {
            Name = name;
            _color = ColorExtensions.FromString(color);
        }
    }
}
