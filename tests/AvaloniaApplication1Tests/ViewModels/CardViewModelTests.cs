using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace AvaloniaApplication1Tests;

public class CardViewModelTests
{
    private readonly IMessageBoxService _messageBoxService = Substitute.For<IMessageBoxService>();

    /// <summary>
    /// Subject under test
    /// </summary>
    private readonly CardViewModel _sut;
    public CardViewModelTests(ITestOutputHelper output)
    {
        var serviceCollection = Ioc.BuildDependencyGraphForTests(output);

        //mock the messagebox service
        serviceCollection.AddSingleton(_messageBoxService);

        _sut = serviceCollection.BuildServiceProvider().GetRequiredService<CardViewModel>();

        /* 
         * you can also create the _sut by yourself like this.
        */
        //_sut = new CardViewModel(_messageBoxService);
    }

    [Fact]
    public void WhenClickButton_Then_DisplayMessageBox()
    {
        _sut.Alert("Hello World!");

        _messageBoxService.Received(1).Show("Hello World!");
    }
}
