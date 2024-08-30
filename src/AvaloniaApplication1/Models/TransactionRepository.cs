using AutoBogus;
using AutoBogus.Conventions;
using Bogus;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Models;

public interface ITransactionRepository
{
    Task<Transaction[]> GetTransactions();
}
public class TransactionRepository(ILogger<ITransactionRepository> _logger) : ITransactionRepository
{
    private readonly Faker<Transaction> _faker = new AutoFaker<Transaction>()
        .Configure(builder => builder.WithConventions())
        .RuleFor(x => x.Id, faker => faker.IndexGlobal)
        .RuleFor(x => x.Merchant, faker => faker.Company.CompanyName())
        .RuleFor(x => x.Category, faker => faker.PickRandom(faker.Commerce.Categories(5)))
        ;
    public async Task<Transaction[]> GetTransactions()
    {
        _logger.LogInformation("GetTransaction()");
        await Task.Delay(3000);
        return _faker.GenerateBetween(10, 30).ToArray();
    }
}
