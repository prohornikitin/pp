using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AvaloniaUi.Converters;
public class ItemToCollectionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        return new ObservableCollection<object>() {
            value
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => throw new NotSupportedException();
}