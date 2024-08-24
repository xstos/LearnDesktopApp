using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1;

internal static class DepensencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<ITransactionRepository, TransactionRepository>();
        services.AddSingleton<WalletViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<WalletView>();
        services.AddTransient<MainWindow>();
        services.AddTransient<StatusBar>();
        return services;
    }
}
