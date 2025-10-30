using CrudPedidos.ViewModels;
using System;
using System.Windows;

namespace CrudPedidos.Views
{
	public partial class PedidoFormWindow : Window
	{
		public PedidoFormWindow()
		{
			InitializeComponent();
			var vm = DataContext as PedidoFormViewModel;
			if (vm != null)
			{
				vm.Saved += (s, e) => DialogResult = true;
				vm.Error += (s, msg) => MessageBox.Show(msg, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}


