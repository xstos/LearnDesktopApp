using AutoBogus;
using Xunit;
using Xunit.Abstractions;

namespace AvaloniaApplication1.Models.Tests;

public class TransactionRepositoryTests
{
    private readonly ITestOutputHelper _output;

    public TransactionRepositoryTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact()]
    public void AutoFakerTest()
    {
        var transactionFaker = new AutoFaker<Transaction>()
            .RuleFor(x => x.Merchant, fake => fake.Company.CompanyName());
        _output.WriteLine(transactionFaker.Generate().ToString());
        _output.WriteLine(transactionFaker.Generate().ToString());
    }

    [Fact()]
    public async Task GetTransactionsTest()
    {
        TransactionRepository sut = new(_output.ToLogger<ITransactionRepository>());
        var transactions = await sut.GetTransactions();
        _output.WriteLine(string.Join("\n", transactions.Select(x => x.ToString())));
    }
}