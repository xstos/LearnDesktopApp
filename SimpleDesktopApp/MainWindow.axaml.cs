using Avalonia.Controls;

namespace SimpleDesktopApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        myButton.Content = "Hello from Avalonia!";
    }
}