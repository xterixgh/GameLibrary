using Avalonia.Controls;
using GameLibrary.Pages;

namespace GameLibrary;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainControl.Content = new Login();
    }
}
