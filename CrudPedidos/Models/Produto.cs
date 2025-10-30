using System;
using System.Globalization;

namespace CrudPedidos.Models
{
	public class Produto
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Codigo { get; set; }
		public string Status { get; set; } = "A";
		public decimal Valor { get; set; }
		

		public Produto()
		{
		}

		public Produto(int id, string nome, string codigo,  decimal valor, string status = "A")
		{
			Id = id;
			Nome = nome;
			Codigo = codigo;
			Status = status;
			Valor = valor;
			Validar();
		}

		public void Validar()
		{
			if (string.IsNullOrWhiteSpace(Nome))
				throw new ArgumentException("Nome é obrigatório.");
			if (string.IsNullOrWhiteSpace(Codigo))
				throw new ArgumentException("Código é obrigatório.");
			if (Valor <= 0)
				throw new ArgumentException("Valor deve ser maior que zero.");
		}

		public static bool TryParseValor(string texto, out decimal valor)
		{
			// aceita vírgula ou ponto
			var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
			var culturePt = new CultureInfo("pt-BR");
			var cultureEn = CultureInfo.InvariantCulture;
			return decimal.TryParse(texto, style, culturePt, out valor) || decimal.TryParse(texto, style, cultureEn, out valor);
		}

		public string StatusDescricao => Status == "A" ? "Ativo" : "Inativo";
	}


}


