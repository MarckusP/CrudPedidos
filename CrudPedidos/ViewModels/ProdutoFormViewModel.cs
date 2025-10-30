using CrudPedidos.Models;
using System;
using System.Globalization;
using System.Windows.Input;

namespace CrudPedidos.ViewModels
{
	public class ProdutoFormViewModel : BaseViewModel
	{
		private int _id;
		private string _nome;
		private string _codigo;
		private decimal _valor;
		private string _valorTexto;

		public int Id { get { return _id; } set { _id = value; RaisePropertyChanged(); } }
		public string Nome { get { return _nome; } set { _nome = value; RaisePropertyChanged(); } }
		public string Codigo { get { return _codigo; } set { _codigo = value; RaisePropertyChanged(); } }
		public decimal Valor { get { return _valor; } set { _valor = value; RaisePropertyChanged(); ValorTexto = _valor.ToString("N2", new CultureInfo("pt-BR")); } }
		public string ValorTexto { get { return _valorTexto; } set { _valorTexto = value; RaisePropertyChanged(); } }

		public ICommand SalvarCommand { get; }
		public event EventHandler Saved;
		public event EventHandler<string> Error;

		public ProdutoFormViewModel(Produto produto = null)
		{
			if (produto != null)
			{
				Id = produto.Id;
				Nome = produto.Nome;
				Codigo = produto.Codigo;
				Valor = produto.Valor;
			}
			else
			{
				Valor = 0m;
				ValorTexto = string.Empty;
			}
			SalvarCommand = new RelayCommand(ExecuteSalvar);
		}

		private void ExecuteSalvar()
		{
			try
			{
				decimal valor;
				if (!Produto.TryParseValor(ValorTexto, out valor))
				{
					throw new ArgumentException("Informe um valor numérico válido.");
				}
				Valor = valor;
				var p = new Produto(Id, Nome, Codigo, Valor);
				Saved?.Invoke(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				Error?.Invoke(this, ex.Message);
			}
		}

		public Produto ToModel()
		{
			return new Produto(Id, Nome, Codigo, Valor);
		}

	}
}


