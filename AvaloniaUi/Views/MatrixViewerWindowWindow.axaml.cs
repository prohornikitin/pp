using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaUi.Models;
using AvaloniaUi.ViewModels;

namespace AvaloniaUi.Views;
public partial class MatrixViewer : UserControl
{
    public MatrixViewer()
    {
        InitializeComponent();
        DataContext = new MatrixViewerVm(new NullMatrixMovingWindow(10));
    }
}