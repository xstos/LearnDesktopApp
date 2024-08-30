using AvaloniaApplication1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.ObjectModel;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly ILogger<MainViewModel> _logger;

    public ObservableCollection<TabItemViewModel> Tabs { get; set; }

    private readonly WalletViewModel _walletViewModel;

    [ObservableProperty]
    private TabItemViewModel? _selectedTabValue;

    public MainViewModel(WalletViewModel walletViewModel, CardViewModel cardViewModel, MenuViewModel menuViewModel, DraftViewModel draftViewModel, ILogger<MainViewModel> logger)
    {
        _logger = logger;
        _walletViewModel = walletViewModel;

        Tabs = new ObservableCollection<TabItemViewModel>
        {
            new TabItemViewModel { Header = "Wallet", Content = walletViewModel },
            new TabItemViewModel { Header = "Card", Content = cardViewModel },
            new TabItemViewModel { Header = "Menu", Content = menuViewModel },
            new TabItemViewModel { Header = "**Draft**", Content = draftViewModel }
        };
    }

    partial void OnSelectedTabValueChanged(TabItemViewModel? tabValue)
    {
        if (tabValue is null)
        {
            return;
        }
        if (tabValue.Content != _walletViewModel)
        {
            return;
        }
        _logger.LogInformation("Wallet Tab selected.");
        Messenger.Send(new ReloadTransactionRequest());
    }
}

public class MainViewModelForDesigner : MainViewModel
{
    public MainViewModelForDesigner() : base(new WalletViewModelForDesigner(),
        new CardViewModelForDesigner(), new MenuViewModelForDesigner(),
        new DraftViewModel(),
        NullLogger<MainViewModelForDesigner>.Instance)
    {

    }
}