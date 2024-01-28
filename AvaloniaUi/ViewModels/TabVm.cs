using AvaloniaUi.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaUi.ViewModels;

public partial class TabVm : VmBase
{
    private enum TabType {
        Generator,
        Viewer,
    };

    [ObservableProperty]
    private string header = "Default Header";


    [ObservableProperty]
    private TabInnerVmBase innerVm;

    [ObservableProperty]
    private string type;

    public delegate void CloseHandler(TabVm sender);
    public event CloseHandler? OnClose;


    [RelayCommand]
    private void Close()
    {
        InnerVm.Dispose();
        OnClose?.Invoke(this);
    }

    
    public static TabVm CreateGeneratorTab() {
        var innerVm = new MatrixGeneratorVm();
        return new TabVm(innerVm, "Generator");
    }

    public static TabVm CreateViewerTab(string filePath)
    {
        var innerVm = new MatrixViewerVm(new MatrixMovingWindow(filePath, 10));
        return new TabVm(innerVm, "Viewer");
    }

    private TabVm(TabInnerVmBase innerVm, string type)
    {
        this.type = type;
        InnerVm = innerVm;
        Header = innerVm.Header;
    }
}