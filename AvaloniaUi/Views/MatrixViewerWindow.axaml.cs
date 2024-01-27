using Avalonia.Controls;
using AvaloniaUi.Models;
using AvaloniaUi.ViewModels;
using MatrixFile;

namespace AvaloniaUi.Views;
public partial class MatrixViewerWindow : Window
{
    public MatrixViewerWindow()
    {
        InitializeComponent();
    }
    public MatrixViewerWindow(string fileName) : this()
    {
        DataContext = new MatrixViewerVm(new FileMatrixMovingWindow(new Matrix(fileName), 10));
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        var vm = DataContext as MatrixViewerVm;
        vm?.Dispose();
        base.OnClosing(e);
    }
}