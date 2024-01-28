using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;


namespace AvaloniaUi.Converters;
public class TemplateDictionaryConverter : Dictionary<string, IDataTemplate>, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s) {
            return this[s];
        }
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => throw new NotSupportedException();
}