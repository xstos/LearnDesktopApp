using AvaloniaApplication1.Models;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaApplication1.ViewModels;

public partial class WalletViewModel : ObservableRecipient, IRecipient<ReloadTransactionRequest>
{
    #region async text
    [ObservableProperty]
    private Task<string> asyncText;

    [ObservableProperty]
    private bool canExecuteReloadAsyncTextCommand;

    partial void OnCanExecuteReloadAsyncTextCommandChanged(bool value)
    {
        this.ReloadAsyncTextCommand.NotifyCanExecuteChanged();
    }

    public async Task<string> FirstLoadAsyncText()
    {
        this.CanExecuteReloadAsyncTextCommand = false;
        await Task.Delay(3000);
        this.CanExecuteReloadAsyncTextCommand = true;
        return "Hello from external service";
    }

    private readonly Faker _faker = new Faker();

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanExecuteReloadAsyncTextCommand))]
    public async Task ReloadAsyncText()
    {
        AsyncText = Task.FromResult("Reloading");
        await Task.Delay(1000);
        AsyncText = Task.FromResult("Reload ok: " + _faker.Company.CompanyName());
    }

    #endregion async text

    [ObservableProperty]
    private Task<Transaction[]> transactions;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LoadingLabel))]
    [NotifyPropertyChangedRecipients]
    private bool isLoadingTranactions = true;

    public string LoadingLabel => IsLoadingTranactions ? "Computed Property: Loading..." : "Computed Property: Loaded";

    /// <summary>
    /// The value is null in designer mode
    /// </summary>
    private readonly ITransactionRepository? _transactionRepository;

    public WalletViewModel(ITransactionRepository? transactionRepository)
    {
        _transactionRepository = transactionRepository;
        AsyncText = FirstLoadAsyncText();
        transactions = LoadTransactions();
        IsActive = true;
    }

    protected virtual async Task<Transaction[]> LoadTransactions()
    {
        IsLoadingTranactions = true;
        try
        {
            return await _transactionRepository!.GetTransactions();
        }
        finally
        {
            IsLoadingTranactions = false;
        }
    }

    public void Receive(ReloadTransactionRequest message)
    {
        if (IsLoadingTranactions)
        {
            return;
        }
        transactions = LoadTransactions();
    }
}

public sealed class WalletViewModelForDesigner : WalletViewModel
{
    public WalletViewModelForDesigner() : base(default)
    {

        AsyncText = Task.FromResult("Async text for Designer Mode");
        IsLoadingTranactions = true;
    }
    protected override async Task<Transaction[]> LoadTransactions()
    {
        await Task.Yield();
        return [
            new Transaction
            {
                Id = 1,
                Amount = 100,
                Merchant = "Sample Merchant 1",
                Date = DateTime.Now
            },
            new Transaction
            {
                Id = 2,
                Amount = 200,
                Merchant = "Sample Merchant 2",
                Date = DateTime.Now
            }
        ];
    }
}
