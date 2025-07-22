using Avalonia;
using Avalonia.Browser;
using AvaloniaApplication1;
using System;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            await BuildAvaloniaApp()
                .WithInterFont().WithMyConfig()
                .LogToTrace()
                .StartBrowserAppAsync("out");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
