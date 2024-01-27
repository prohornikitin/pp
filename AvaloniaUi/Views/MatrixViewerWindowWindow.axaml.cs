using Avalonia.Controls;
using Avalonia.Interactivity;
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

    protected override void OnPropertyChanged(Avalonia.AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DataContextProperty) {
            var oldVm = change.OldValue as MatrixViewerVm;
            oldVm?.Dispose();
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        var vm = DataContext as MatrixViewerVm;
        vm?.Dispose();
    }
}