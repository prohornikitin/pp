using Avalonia.Controls;
using AvaloniaUi.ViewModels;

namespace AvaloniaUi.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainVm();
    }
}