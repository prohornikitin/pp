using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaUi.CustomControls;
public class IntTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    protected override void OnTextInput(TextInputEventArgs e)
    {
        if (!int.TryParse(Text + e.Text!, out _))
        {
            e.Handled = true;
        }
        else
        {
            base.OnTextInput(e);
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if(Text == null || Text.Length == 0)
        {
            e.Handled = true;
            Text = "0";
        }
        if(!int.TryParse(Text, out _))
        {
            e.Handled = true;
            Undo();
        }
        Text = Text.TrimStart('0');
    }
}