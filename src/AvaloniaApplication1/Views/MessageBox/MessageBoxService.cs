using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.ViewModels;

internal class MessageBoxService(MessageBoxViewModel _messageBoxViewModel) : IMessageBoxService
{
    public void Show(string message)
    {
        _messageBoxViewModel.Message = message;
        DialogHostAvalonia.DialogHost.Show(_messageBoxViewModel);
    }
}
