using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;

namespace SpeckleRevitPlugin.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectionChangedToDataStreamConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            var items = ((SelectionChangedEventArgs) value).AddedItems;
            return items.Count > 0 ? items[0] : null;
        }
    }

    /// <summary>
    /// Inverse of the BooleanToVisibilityConverter. Hides elements when Boolean is True.
    /// </summary>
    public class BooleanToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
