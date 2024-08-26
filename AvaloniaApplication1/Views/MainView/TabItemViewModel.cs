using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

public class TabItemViewModel
{
    public string Header { get; set; }
    public ObservableObject Content { get; set; }
}
