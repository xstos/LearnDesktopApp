using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace AvaloniaApplication1;

public class ViewLocator : IDataTemplate
{
    private static readonly ImmutableDictionary<Type, Type> ViewModelToControlMap = new Dictionary<Type, Type>()
    {
        //{ typeof(WalletViewModel), typeof(WalletView) },
        //{ typeof(MainViewModel), typeof(MainView) },
        //{ typeof(StatusBarViewModel), typeof(StatusBar) }
    }.ToImmutableDictionary();

    public Control Build(object viewModel)
    {
        ArgumentNullException.ThrowIfNull(App.ServiceProvider, nameof(App.ServiceProvider));
        var viewModelType = viewModel.GetType();
        if (ViewModelToControlMap.TryGetValue(viewModelType, out var controlType) && controlType is not null)
        {
            var control = App.ServiceProvider.GetRequiredService(controlType);
            if (control is Control c)
            {
                return c;
            }
            return NotFound($"The type {controlType.FullName} is not a Control");
        }

        var viewModelFullTypeName = viewModelType.FullName;
        if (string.IsNullOrEmpty(viewModelFullTypeName)) { return NotFound("Null"); }

        if (viewModel is not ObservableObject)
        {
            return NotFound($"{viewModelFullTypeName} is not a ViewModel");
        }
        foreach (var controlFullTypeName in PossibleControlFullTypeNames(viewModelFullTypeName))
        {
            var controlType1 = Type.GetType(controlFullTypeName);
            if (controlType1 is null) { continue; }
            var control = App.ServiceProvider.GetService(controlType1);
            if (control is Control c)
            {
                Console.WriteLine($"Found control: {controlFullTypeName}");
                return c;
            }
        }

        return NotFound($"No corresponding control found for {viewModelFullTypeName}");
    }

    /// <summary>
    /// if viewModelFullTypeName is "ViewModels.WalletViewModel" or "ViewModels.Wallet" or "ViewModels.WalletViewModelForDesigner" then this method will yield
    /// the possible corresponding Control which are ["Views.WalletView", "Views.WalletControl", "Views.Wallet"]
    /// </summary>
    /// <param name="viewModelFullTypeName"></param>
    /// <returns>possibles controlFullTypeName</returns>
    public static IEnumerable<string> PossibleControlFullTypeNames(string viewModelFullTypeName)
    {
        var controlName = viewModelFullTypeName;
        if (viewModelFullTypeName.EndsWith("ViewModelForDesigner"))
        {
            controlName = viewModelFullTypeName.Substring(0, viewModelFullTypeName.Length - 20);
        }
        else if (viewModelFullTypeName.EndsWith("ViewModel"))
        {
            controlName = viewModelFullTypeName.Substring(0, viewModelFullTypeName.Length - 9);
        }
        controlName = controlName.Replace(".ViewModel", ".View");
        yield return controlName + "View";
        yield return controlName + "Control";
        if (controlName != viewModelFullTypeName)
        {
            yield return controlName;
        }
    }
    private Control NotFound(string message) => new TextBlock { Text = "Control not found. " + message };

    public bool Match(object data) => data is ObservableObject;
}
