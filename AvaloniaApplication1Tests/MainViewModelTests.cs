using AvaloniaApplication1;
using AvaloniaApplication1.ViewModels;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AvaloniaApplication1Tests;

public class MainViewModelTests
{
    private readonly MainViewModel _sut;

    public MainViewModelTests(ITestOutputHelper output)
    {
        //replace the application logger (seq, console..) with the xunit logger
        //so that we could see application's log in the test output
        App.ServiceProvider.RegisterInstance(output.ToLoggerFactory(), ifAlreadyRegistered: IfAlreadyRegistered.Replace);

        //replace external services with mock (otherwise the tests will use real services)
        //App.ServiceProvider.RegisterInstance<ITransactionRepository>(new MockTransactionRepository(), ifAlreadyRegistered: IfAlreadyRegistered.Replace);

        //We modified the IoC container, make the test failed if it becomes invalid
        App.ServiceProvider.ValidateAndThrow();

        _sut = App.ServiceProvider.GetRequiredService<MainViewModel>();
    }


    [Fact]
    public void OnSelectedTabValueChanged()
    {
        _sut.ShouldNotBeNull();
    }
}
