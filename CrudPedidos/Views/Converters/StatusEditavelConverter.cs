using System;
using System.Globalization;
using System.Windows.Data;
using CrudPedidos.Models;

namespace CrudPedidos.Converters
{
    public class StatusEditavelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StatusPedido s)
                return s != StatusPedido.Recebido; 
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
