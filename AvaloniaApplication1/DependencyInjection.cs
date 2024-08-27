using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        services.AddLogging(builder
            => builder.AddConsole()
                .AddDebug()
                .AddSeq()
                .SetMinimumLevel(LogLevel.Debug));
        return services;
    }

    /// <summary>
    /// Registration for application
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IContainer RegisterApplicationServices(this IContainer services)
    {
        services.Register<ITransactionRepository, TransactionRepository>(Reuse.Singleton);

        services.Register<MainWindowViewModel>(Reuse.Singleton);
        services.Register<MainWindow>();

        services.Register<MainViewModel>(Reuse.Singleton);
        services.Register<MainView>();

        services.Register<CardViewModel>(Reuse.Singleton);
        services.Register<CardView>();

        services.Register<MenuViewModel>(Reuse.Singleton);
        services.Register<MenuView>();

        services.Register<WalletViewModel>(Reuse.Singleton);
        services.Register<WalletView>();

        services.Register<StatusBar>();
        return services;
    }
}
