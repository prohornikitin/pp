using Avalonia.Controls;
using AvaloniaUi.ViewModels;

namespace AvaloniaUi.Views;
public partial class MatrixGeneratorWindow : Window
{
    public MatrixGeneratorWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        var vm = DataContext as MatrixGeneratorVm;
        vm?.StopGeneration();
        base.OnClosing(e);
    }
}