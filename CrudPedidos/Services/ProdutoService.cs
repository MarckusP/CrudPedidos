using CrudPedidos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrudPedidos.Services
{
	public class ProdutoService
	{
		private readonly string _jsonPath;
		private readonly List<Produto> _produtos;

		public ProdutoService(string path = null)
		{
			var baseDir = AppDomain.CurrentDomain.BaseDirectory;

			// Volta até a raiz do projeto (saindo de bin/Debug/net4.6/)
			var projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\"));

			_jsonPath = string.IsNullOrWhiteSpace(path)
				? Path.Combine(projectRoot, "Data", "produto.json")
				: path;

			_produtos = CarregarInterno();
		}

		public IReadOnlyList<Produto> ObterTodos()
		{
			return _produtos.OrderBy(p => p.Id).ToList();
		}

		public List<Produto> Filtrar(string nome, string codigo, decimal? valorMin, decimal? valorMax, bool incluirInativos = false, string status = null)
		{
			IEnumerable<Produto> query = _produtos;
			if (!incluirInativos) query = query.Where(p => p.Status == "A");
			if (!string.IsNullOrWhiteSpace(nome))
				query = query.Where(p => (p.Nome ?? string.Empty).ToLowerInvariant().Contains(nome.Trim().ToLowerInvariant()));
			if (!string.IsNullOrWhiteSpace(codigo))
				query = query.Where(p => (p.Codigo ?? string.Empty).ToLowerInvariant().Contains(codigo.Trim().ToLowerInvariant()));
			if (valorMin.HasValue)
				query = query.Where(p => p.Valor >= valorMin.Value);
			if (valorMax.HasValue)
				query = query.Where(p => p.Valor <= valorMax.Value);
			if (!string.IsNullOrWhiteSpace(status) && status != "Ambos")
			{
				string s = status == "Ativo" ? "A" : "I";
				query = query.Where(p => p.Status == s);
			}
			return query.OrderBy(p => p.Id).ToList();
		}

		public Produto Adicionar(string nome, string codigo, decimal valor)
		{
			if (_produtos.Any(p => string.Equals(p.Codigo, codigo, StringComparison.OrdinalIgnoreCase)))
				throw new InvalidOperationException("Código já cadastrado.");
			var produto = new Produto(ObterProximoId(), nome, codigo, valor);
			_produtos.Add(produto);
			Salvar();
			return produto;
		}

		public void Atualizar(Produto atualizado)
		{
			if (atualizado == null) throw new ArgumentNullException(nameof(atualizado));
			atualizado.Validar();
			if (_produtos.Any(p => p.Id != atualizado.Id && string.Equals(p.Codigo, atualizado.Codigo, StringComparison.OrdinalIgnoreCase)))
				throw new InvalidOperationException("Código já cadastrado.");
			var idx = _produtos.FindIndex(p => p.Id == atualizado.Id);
			if (idx < 0) throw new InvalidOperationException("Produto não encontrado.");
			_produtos[idx] = atualizado;
			Salvar();
		}

		public void AlterarStatus(int id, string status)
		{
			var existente = _produtos.FirstOrDefault(p => p.Id == id);
			if (existente == null) return;
			existente.Status = status == "A" ? "A" : "I";
			Salvar();
		}

		private int ObterProximoId()
		{
			return _produtos.Count == 0 ? 1 : _produtos.Max(p => p.Id) + 1;
		}

		private List<Produto> CarregarInterno()
		{
			try
			{
				var dir = Path.GetDirectoryName(_jsonPath);
				if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
				if (!File.Exists(_jsonPath)) return new List<Produto>();
				string json = File.ReadAllText(_jsonPath);
				var obj = JsonConvert.DeserializeObject<List<Produto>>(json);
				return obj ?? new List<Produto>();
			}
			catch
			{
				return new List<Produto>();
			}
		}

		private void Salvar()
		{
			var directory = Path.GetDirectoryName(_jsonPath);
			if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
			string json = JsonConvert.SerializeObject(_produtos, Formatting.Indented);
			File.WriteAllText(_jsonPath, json);
		}

		public void Excluir(int produtoId)
		{
			var produto = _produtos.FirstOrDefault(p => p.Id == produtoId);
			if (produto == null)
				throw new InvalidOperationException("Produto não encontrado.");

			_produtos.Remove(produto);
			Salvar();
		}
	}
}
