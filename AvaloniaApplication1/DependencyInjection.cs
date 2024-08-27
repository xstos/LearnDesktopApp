using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1;

internal static class DependencyInjection
{
    /// <summary>
    /// Registration for Microsoft Logging framework
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddLoggingService(this IServiceCollection services)
    {
        services.AddLogging();
        return services;
    }

    /// <summary>
    /// Registration for application
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static ServiceCollection RegisterApplicationServices(this ServiceCollection services)
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

        services.AddTransient<WalletViewModel>();
        services.AddTransient<WalletView>();

        services.AddTransient<StatusBar>();
        return services;
    }
}
