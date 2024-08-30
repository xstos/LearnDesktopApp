using AvaloniaApplication1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AvaloniaApplication1Tests;

static class Ioc
{
    public static ServiceCollection BuildDependencyGraphForTests(ITestOutputHelper output)
    {
        var serviceCollection = App.BuildDependencyGraph();

        //replace the application logger (seq, console..) with the xunit logger
        //so that we could see application's log in the test output
        serviceCollection.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddXUnit(output);
        });

        return serviceCollection;
    }
}
