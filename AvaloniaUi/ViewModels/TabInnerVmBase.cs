using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaUi.ViewModels;

public abstract partial class TabInnerVmBase : VmBase, IDisposable {
    [ObservableProperty]
    private string header = "Tab";

    public abstract void Dispose();
}