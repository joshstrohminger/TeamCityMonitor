using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using BlinkStickUniversal;
using Interfaces;
using MVVM.Annotations;

namespace DesignData
{
    public class LabeledColorDesignData : ILabeledColor
    {
        public string Name { get; }
        public Color Color { get; set; }

        public LabeledColorDesignData()
        {
        }

        public LabeledColorDesignData(string name, string color)
        {
            Name = name;
            Color = ColorExtensions.FromString(color);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
