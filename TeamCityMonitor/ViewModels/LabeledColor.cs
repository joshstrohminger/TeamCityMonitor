using System.Runtime.Serialization;
using Windows.UI;
using BlinkStickUniversal;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.ViewModels
{
    [DataContract]
    public class LabeledColor : ObservableObject, ILabeledColor
    {
        private Color _color;

        [DataMember]
        public string Name { get; protected set; }

        [DataMember]
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
