using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AvaloniaApplication1Tests;

public class DryIocIntegrationTests(ITestOutputHelper _outputHelper)
{
    [Fact]
    public void MsDryIoc_CombinedRegistrationsTest()
    {
        //Microsoft-things must to be registered first
        ServiceCollection services = new();
        services.AddLogging(builder
            => builder.AddConsole()
                .AddXUnit(_outputHelper)
                .SetMinimumLevel(LogLevel.Debug));

        //Everything-else registration is added on top of the Microsoft-things
        var container = new DryIocServiceProviderFactory().CreateBuilder(services);
        container.Register<IDep, X>();
        container.GetRequiredService<IDep>().ShouldBeOfType<X>();

        //Replace the X implementation with a different one (a Mock in the test for eg)
        container.Register<IDep, Y>(ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        container.GetRequiredService<IDep>().ShouldBeOfType<Y>();

        //Resolve Microsoft-things works too
        var logger = container.GetRequiredService<ILogger<IDep>>();
        logger.LogInformation("alright");
    }

    interface IDep;
    class X : IDep;
    class Y : IDep;
}
