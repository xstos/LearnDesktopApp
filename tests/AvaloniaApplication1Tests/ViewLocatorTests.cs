using AvaloniaApplication1;
using Shouldly;
using Xunit;

namespace AvaloniaApplication1Tests;

public class ViewLocatorTests
{
    [Fact]
    public void GetPossibleControlFullTypeNamesTest()
    {
        var candidateControls_1 = ViewLocator.PossibleControlFullTypeNames("App.ViewModels.Wallet");
        var candidateControls_2 = ViewLocator.PossibleControlFullTypeNames("App.ViewModels.WalletViewModel");
        var candidateControls_3 = ViewLocator.PossibleControlFullTypeNames("App.ViewModels.WalletViewModelForDesigner");
        candidateControls_1.ShouldBe(candidateControls_2, ignoreOrder: true);
        candidateControls_1.ShouldBe(candidateControls_3, ignoreOrder: true);
        candidateControls_1.ShouldBe(["App.Views.Wallet", "App.Views.WalletView", "App.Views.WalletControl"], ignoreOrder: true);
    }
}
