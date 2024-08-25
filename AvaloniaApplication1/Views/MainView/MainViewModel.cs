using Avalonia.Controls;
using AvaloniaApplication1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private WalletViewModel _walletViewModel;
    [ObservableProperty]
    private CardViewModel _cardViewModel;
    [ObservableProperty]
    private MenuViewModel _menuViewModel;
    private readonly ILogger<MainViewModel> _logger;

    [ObservableProperty]
    private TabItem? _selectedTabItem;

    public MainViewModel(WalletViewModel walletViewModel, CardViewModel cardViewModel, MenuViewModel menuViewModel, ILogger<MainViewModel> logger)
    {
        _walletViewModel = walletViewModel;
        _cardViewModel = cardViewModel;
        _menuViewModel = menuViewModel;
        _logger = logger;
    }

    partial void OnSelectedTabItemChanged(TabItem? tabItem)
    {
        if (tabItem is null)
        {
            return;
        }
        if ((tabItem.Content as ContentControl)?.Content != WalletViewModel)
        {
            return;
        }
        _logger.LogInformation("Wallet Tab selected {0}");
        Messenger.Send(new ReloadTransactionRequest());
    }
}
