using CommunityToolkit.Mvvm.ComponentModel;

namespace ReactiveDesktopApp.ViewModels;

public partial class UserControl1ViewModel : ObservableObject
{
    [ObservableProperty]
    private string _taskName = "Task 1";

}