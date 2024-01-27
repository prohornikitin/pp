using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaUi.ViewModels;

public partial class VmBase : ObservableObject
{
    protected void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if(EqualityComparer<T>.Default.Equals(field, value)) {
            return;
        }
        field = value;
        OnPropertyChanged(propertyName);
    }
}
