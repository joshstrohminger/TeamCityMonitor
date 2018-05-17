using System.ComponentModel;
using Windows.UI;

namespace Interfaces
{
    public interface ILabeledColor : INotifyPropertyChanged
    {
        string Name { get; }
        Color Color { get; set; }
    }
}