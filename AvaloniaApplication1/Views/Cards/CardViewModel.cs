using AvaloniaApplication1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class CardViewModel(IMessageBoxService _messageBoxService) : ObservableObject
{
    [RelayCommand]
    public void Alert(string message)
    {
        _messageBoxService.Show(message);
    }
}

public class CardViewModelForDesigner : CardViewModel
{
    public CardViewModelForDesigner() : base(new MessageBoxService(new MessageBoxViewModel()))
    {
    }
}
