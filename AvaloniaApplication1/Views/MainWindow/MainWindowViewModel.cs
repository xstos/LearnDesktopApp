using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private MainViewModel mainViewModel;

    public MainWindowViewModel(MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel;
    }
}
