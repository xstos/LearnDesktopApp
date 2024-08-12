using Avalonia.Controls;
using ReactiveDesktopApp.ViewModels;

namespace ReactiveDesktopApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.UserControl1.DataContext = new UserControl1ViewModel();
    }
}