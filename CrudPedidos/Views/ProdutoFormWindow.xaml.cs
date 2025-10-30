using CrudPedidos.ViewModels;
using System;
using System.Windows;

namespace CrudPedidos.Views
{
	public partial class ProdutoFormWindow : Window
	{
		public ProdutoFormWindow()
		{
			InitializeComponent();
			var vm = DataContext as ProdutoFormViewModel;
			if (vm != null)
			{
				vm.Saved += Vm_Saved;
				vm.Error += Vm_Error;
			}
		}

		private void Vm_Saved(object sender, EventArgs e)
		{
			DialogResult = true;
		}

		private void Vm_Error(object sender, string e)
		{
			MessageBox.Show(e, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
		}
	}
}


