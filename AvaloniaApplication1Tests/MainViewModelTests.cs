using AvaloniaApplication1;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;
using DryIoc;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace AvaloniaApplication1Tests;

public class MainViewModelTests
{
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    private MainViewModel _sut;

    public MainViewModelTests(ITestOutputHelper output)
    {
        //replace the application logger (seq, console..) with the xunit logger
        //so that we could see application's log in the test output
        App.ServiceProvider.RegisterInstance(output.ToLoggerFactory(), ifAlreadyRegistered: IfAlreadyRegistered.Replace);
    }


    [Fact]
    public void When_WalletTab_IsSelected_Then_TransactionList_ShouldBe_Reloaded()
    {
        //Arrange

        IContainer? containerForTest = App.ServiceProvider?.Clone();

        //replace external services with mock (otherwise the tests will use real services)
        containerForTest?.RegisterInstance(_transactionRepository, ifAlreadyRegistered: IfAlreadyRegistered.Replace);

        _sut = containerForTest?.Resolve<MainViewModel>();

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
