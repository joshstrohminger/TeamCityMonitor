using BlinkStickUniversal;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.ViewModels
{
    public class LabeledColor : ObservableObject, ILabeledColor
    {
        private RgbColor _color;

        public string Name { get; }

        public RgbColor Color
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
            _color = RgbColor.FromString(color);
        }
    }
}
