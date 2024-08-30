using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.ViewModels;

public partial class CardViewModel : ObservableObject
{
    [RelayCommand]
    public void Alert()
    {
        MessageBoxViewModel messageBoxViewModel = App.ServiceProvider!.GetRequiredService<MessageBoxViewModel>();
        messageBoxViewModel.Message = "Hello World!";
        DialogHostAvalonia.DialogHost.Show(messageBoxViewModel);
    }
}

public class CardViewModelForDesigner : CardViewModel;
