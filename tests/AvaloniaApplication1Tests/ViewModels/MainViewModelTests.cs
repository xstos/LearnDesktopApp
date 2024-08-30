using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace AvaloniaApplication1Tests;

public class MainViewModelTests
{
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();

    /// <summary>
    /// Subject under test
    /// </summary>
    private readonly MainViewModel _sut;

    public MainViewModelTests(ITestOutputHelper output)
    {
        var serviceCollection = Ioc.BuildDependencyGraphForTests(output);

        //mock the transaction repository
        serviceCollection.AddSingleton(_transactionRepository);

        _sut = serviceCollection.BuildServiceProvider().GetRequiredService<MainViewModel>();
    }

    [Fact]
    public void When_WalletTab_IsSelected_Then_TransactionList_ShouldBe_Reloaded()
    {
        //Assert that GetTransactions is called on start up
        _transactionRepository.Received(1).GetTransactions();
        _transactionRepository.ClearReceivedCalls();

        //Assert that GetTransactions is not called when switch to the second tab
        _sut.SelectedTabValue = _sut.Tabs[1]; //switch to the second tab
        _transactionRepository.DidNotReceive().GetTransactions();
        _transactionRepository.ClearReceivedCalls();

        //Assert that GetTransactions is called when switch back to the first tab
        _sut.SelectedTabValue = _sut.Tabs[0]; //switch back to the first tab
        _transactionRepository.Received(1).GetTransactions();
    }
}
