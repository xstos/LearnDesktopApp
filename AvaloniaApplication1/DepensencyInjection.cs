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


        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();

        services.AddSingleton<MainViewModel>();
        services.AddTransient<MainView>();

        services.AddSingleton<CardViewModel>();
        services.AddTransient<CardView>();


        services.AddSingleton<MenuViewModel>();
        services.AddTransient<MenuView>();

        services.AddSingleton<WalletViewModel>();
        services.AddTransient<WalletView>();

        services.AddTransient<StatusBar>();
        return services;
    }
}
