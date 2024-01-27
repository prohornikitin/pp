using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaUi.ViewModels;

public partial class ItemVm : VmBase
{
    [ObservableProperty]
    private string text = "";

    [ObservableProperty]
    public bool meaningfull = true;

    public string FontWeight => Meaningfull ? "Normal" : "Bold";

    public override bool Equals(object? obj)
    {
        var objTyped = obj as ItemVm;
        if (objTyped == null)
        {
            return false;
        }
        
        return objTyped.Text == Text;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(Meaningfull)) {
            OnPropertyChanged(nameof(FontWeight));
        }
        base.OnPropertyChanged(e);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}