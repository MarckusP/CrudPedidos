using CrudPedidos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrudPedidos.Services
{
	public class PedidoService
	{
		private readonly string _jsonPath;
		private readonly List<Pedido> _pedidos;

		public PedidoService(string path = null)
		{
			var baseDir = AppDomain.CurrentDomain.BaseDirectory;

			// Volta para a raiz do projeto (saindo de bin/Debug/net4.6/, por exemplo)
			var projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\"));

			_jsonPath = string.IsNullOrWhiteSpace(path)
				? Path.Combine(projectRoot, "Data", "pedido.json")
				: path;

			_pedidos = CarregarInterno();
		}

		public IReadOnlyList<Pedido> ObterTodos()
		{
			return _pedidos.OrderByDescending(p => p.Id).ToList();
		}

		public Pedido Adicionar(Pedido pedido)
		{
			if (pedido == null) throw new ArgumentNullException(nameof(pedido));
			pedido.Id = ObterProximoId();
			pedido.DataVenda = DateTime.Now;
			pedido.Status = StatusPedido.Pendente;
			pedido.Validar();
			_pedidos.Add(pedido);
			Salvar();
			return pedido;
		}

		public void Atualizar(Pedido pedido)
		{
			if (pedido == null) throw new ArgumentNullException(nameof(pedido));
			pedido.Validar();
			var idx = _pedidos.FindIndex(p => p.Id == pedido.Id);
			if (idx < 0) throw new InvalidOperationException("Pedido não encontrado.");
			_pedidos[idx] = pedido;
			Salvar();
		}

		private int ObterProximoId()
		{
			return _pedidos.Count == 0 ? 1 : _pedidos.Max(p => p.Id) + 1;
		}

		private List<Pedido> CarregarInterno()
		{
			try
			{
				var dir = Path.GetDirectoryName(_jsonPath);
				if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
				if (!File.Exists(_jsonPath)) return new List<Pedido>();
				string json = File.ReadAllText(_jsonPath);
				var obj = JsonConvert.DeserializeObject<List<Pedido>>(json);
				return obj ?? new List<Pedido>();
			}
			catch
			{
				return new List<Pedido>();
			}
		}

		private void Salvar()
		{
			var directory = Path.GetDirectoryName(_jsonPath);
			if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
			string json = JsonConvert.SerializeObject(_pedidos, Formatting.Indented);
			File.WriteAllText(_jsonPath, json);
		}

		public void Excluir(int id)
		{
			var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
			if (pedido == null)
				throw new InvalidOperationException("Pedido não encontrado.");

			_pedidos.Remove(pedido);
			Salvar();
		}

	}
}


