using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaUi.ViewModels;

namespace AvaloniaUi.Views;
public partial class MatrixGenerator : UserControl
{
    public MatrixGenerator()
    {
        InitializeComponent();
        DataContext = new MatrixGeneratorVm();
    }
}