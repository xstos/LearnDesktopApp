using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private WalletViewModel _walletViewModel;
    [ObservableProperty]
    private CardViewModel _cardViewModel;
    [ObservableProperty]
    private MenuViewModel _menuViewModel;

    public MainViewModel(WalletViewModel walletViewModel, CardViewModel cardViewModel, MenuViewModel menuViewModel)
    {
        _walletViewModel = walletViewModel;
        _cardViewModel = cardViewModel;
        _menuViewModel = menuViewModel;
    }
}
