using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaApplication1.ViewModels;

public partial class StatusBarViewModel : ObservableRecipient, IRecipient<PropertyChangedMessage<bool>>
{
    [ObservableProperty]
    private string status;

    public StatusBarViewModel()
    {
        IsActive = true;
    }

    public void Receive(PropertyChangedMessage<bool> message)
    {
        if (message.Sender is WalletViewModel && message.PropertyName == nameof(WalletViewModel.IsLoadingTranactions))
        {
            Status = message.NewValue ? "Status: Loading" : "Status: Loaded";
        }
    }
}

public class StatusBarViewModelForDesigner : StatusBarViewModel
{
    public StatusBarViewModelForDesigner() : base()
    {
        Status = "Status: Fake status for Designer Mode";
    }
}
