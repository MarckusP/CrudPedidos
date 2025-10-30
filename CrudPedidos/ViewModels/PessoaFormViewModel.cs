using CrudPedidos.Models;
using System;
using System.Windows.Input;

namespace CrudPedidos.ViewModels
{
	public class PessoaFormViewModel : BaseViewModel
	{
		private int _id;
		private string _nome;
		private string _cpf;

		public int Id
		{
			get { return _id; }
			set { _id = value; RaisePropertyChanged(); }
		}

		public string Nome
		{
			get { return _nome; }
			set { _nome = value; RaisePropertyChanged(); }
		}

		public string Cpf
		{
			get { return _cpf; }
			set { _cpf = value; RaisePropertyChanged(); }
		}

		public ICommand SalvarCommand { get; }

		public event EventHandler Saved;

		public PessoaFormViewModel(Pessoa pessoa = null)
		{
			if (pessoa != null)
			{
				Id = pessoa.Id;
				Nome = pessoa.Nome;
				Cpf = pessoa.Cpf;
			}
			SalvarCommand = new RelayCommand(ExecuteSalvar);
		}

		private void ExecuteSalvar()
		{
			var p = new Pessoa(Id, Nome, Pessoa.SomenteNumeros(Cpf ?? string.Empty));
			// se não lançar, está válido
			Cpf = p.Cpf; // normaliza sem máscara
			Saved?.Invoke(this, EventArgs.Empty);
		}

		public Pessoa ToModel()
		{
			return new Pessoa(Id, Nome, Pessoa.SomenteNumeros(Cpf ?? string.Empty));
		}
	}
}


