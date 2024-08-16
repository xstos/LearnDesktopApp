using Avalonia;
using Avalonia.Controls;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Views;

public partial class StatusBar : UserControl
{
    public string StatusBarMessage
    {
        get => (DataContext as StatusBarViewModel).Status;
        set => (DataContext as StatusBarViewModel).Status = value;
    }

    public static readonly StyledProperty<string> StatusBarMessageProperty =
       AvaloniaProperty.Register<StatusBar, string>(nameof(StatusBarMessage), defaultValue: "not set");

    public StatusBar()
    {
        DataContext = new StatusBarViewModelForDesigner();
        InitializeComponent();
    }
}