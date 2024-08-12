using Microsoft.Extensions.DependencyInjection;
using ReactiveDesktopApp.ViewModels;

namespace ReactiveDesktopApp;

public static class Dependencies
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<UserControl1ViewModel>();
        return services;
    }
}
