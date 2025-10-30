using CrudPedidos.Models;
using CrudPedidos.ViewModels;
using System;
using System.Windows;

namespace CrudPedidos.Views
{
	public partial class PessoaFormWindow : Window
	{
		public Pessoa Pessoa { get; private set; }

		public PessoaFormWindow(Pessoa pessoa = null)
		{
			InitializeComponent();
			var vm = new PessoaFormViewModel(pessoa);
			vm.Saved += Vm_Saved;
			DataContext = vm;
		}

		private void Vm_Saved(object sender, EventArgs e)
		{
			try
			{
				var vm = (PessoaFormViewModel)DataContext;
				Pessoa = vm.ToModel();
				DialogResult = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}


