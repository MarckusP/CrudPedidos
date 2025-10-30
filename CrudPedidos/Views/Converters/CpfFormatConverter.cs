using System;
using System.Globalization;
using System.Windows.Data;
using CrudPedidos.Models;

namespace CrudPedidos.Views.Converters
{
	public class CpfFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string cpf = value as string;
			return Pessoa.FormatCpf(cpf);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = value as string;
			return Pessoa.SomenteNumeros(text ?? string.Empty);
		}
	}
}


