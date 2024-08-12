using AvaloniaApplication1.Models;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ObservableObject
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
    /// <summary>
    /// The value is null in designer mode
    /// </summary>
    private readonly ITransactionRepository? _transactionRepository;

    public MainViewModel(ITransactionRepository? transactionRepository)
    {
        _transactionRepository = transactionRepository;
        AsyncText = FirstLoadAsyncText();
        if (_transactionRepository is not null)
        {
            transactions = _transactionRepository.GetTransactions();
        }
    }
}
public sealed class MainViewModelForDesigner : MainViewModel
{
    public MainViewModelForDesigner() : base(default)
    {
        Transactions = Task.FromResult<Transaction[]>([
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
        ]);
        AsyncText = Task.FromResult("Fake external service result for Designer Mode");
    }
}
