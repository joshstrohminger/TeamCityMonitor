using System.Windows.Input;

namespace MVVM
{
    public interface IRelayCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}