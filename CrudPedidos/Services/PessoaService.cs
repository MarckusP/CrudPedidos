using CrudPedidos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CrudPedidos.Services
{
    public class PessoaService
    {
        private readonly string _jsonPath;
        private readonly List<Pessoa> _pessoas;

        public PessoaService(string path = null)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\"));

            _jsonPath = string.IsNullOrWhiteSpace(path)
                ? Path.Combine(projectRoot, "Data", "pessoa.json")
                : path;

            _pessoas = CarregarInterno();
        }

        public IReadOnlyList<Pessoa> ObterTodos()
        {
            return _pessoas.OrderBy(p => p.Id).ToList();
        }

        public Pessoa Adicionar(string nome, string cpf)
        {
            string cpfDigits = Pessoa.SomenteNumeros(cpf);
            if (_pessoas.Any(p => Pessoa.SomenteNumeros(p.Cpf) == cpfDigits))
                throw new InvalidOperationException("CPF já cadastrado.");

            var pessoa = new Pessoa(ObterProximoId(), nome, cpfDigits, "A");
            _pessoas.Add(pessoa);
            Salvar();
            return pessoa;
        }

        public void Atualizar(Pessoa pessoaAtualizada)
        {
            if (pessoaAtualizada == null) throw new ArgumentNullException(nameof(pessoaAtualizada));
            pessoaAtualizada.Cpf = Pessoa.SomenteNumeros(pessoaAtualizada.Cpf);
            pessoaAtualizada.Validar();

            string cpfDigits = pessoaAtualizada.Cpf;
            if (_pessoas.Any(p => p.Id != pessoaAtualizada.Id && Pessoa.SomenteNumeros(p.Cpf) == cpfDigits))
                throw new InvalidOperationException("CPF já cadastrado.");

            var idx = _pessoas.FindIndex(p => p.Id == pessoaAtualizada.Id);
            if (idx < 0) throw new InvalidOperationException("Pessoa não encontrada.");

            _pessoas[idx] = pessoaAtualizada;
            Salvar();
        }

        public void AlterarStatus(int id, string status)
        {
            var existente = _pessoas.FirstOrDefault(p => p.Id == id);
            if (existente == null) return;
            existente.Status = status == "A" ? "A" : "I";
            Salvar();
        }

        public List<Pessoa> Filtrar(string nome, string cpf, bool incluirInativos = false, string status = null)
        {
            string nomeFiltro = string.IsNullOrWhiteSpace(nome) ? null : nome.Trim().ToLowerInvariant();
            string cpfFiltro = string.IsNullOrWhiteSpace(cpf) ? null : Pessoa.SomenteNumeros(cpf);
            IEnumerable<Pessoa> query = _pessoas;

            if (!incluirInativos) query = query.Where(p => p.Status == "A");
            if (nomeFiltro != null) query = query.Where(p => (p.Nome ?? string.Empty).ToLowerInvariant().Contains(nomeFiltro));
            if (cpfFiltro != null) query = query.Where(p => Pessoa.SomenteNumeros(p.Cpf ?? string.Empty).Contains(cpfFiltro));
            if (!string.IsNullOrWhiteSpace(status) && status != "Ambos")
            {
                string s = status == "Ativo" ? "A" : "I";
                query = query.Where(p => p.Status == s);
            }

            return query.OrderBy(p => p.Id).ToList();
        }

        private int ObterProximoId()
        {
            return _pessoas.Count == 0 ? 1 : _pessoas.Max(p => p.Id) + 1;
        }

        private List<Pessoa> CarregarInterno()
        {
            try
            {
                if (!File.Exists(_jsonPath)) return new List<Pessoa>();
                string json = File.ReadAllText(_jsonPath);
                var obj = JsonConvert.DeserializeObject<List<Pessoa>>(json);
                return obj ?? new List<Pessoa>();
            }
            catch
            {
                return new List<Pessoa>();
            }
        }

        private void Salvar()
        {
            var directory = Path.GetDirectoryName(_jsonPath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

            foreach (var p in _pessoas)
            {
                p.Cpf = Pessoa.SomenteNumeros(p.Cpf ?? string.Empty);
            }

            string json = JsonConvert.SerializeObject(_pessoas, Formatting.Indented);
            File.WriteAllText(_jsonPath, json);
        }

        public void Excluir(int pessoaId, IEnumerable<Pedido> pedidos)
        {
            // Verifica se existe algum pedido associado a esta pessoa
            if (pedidos.Any(p => p.PessoaId == pessoaId))
                throw new InvalidOperationException("Não é possível excluir esta pessoa: existem pedidos associados.");

            // Remove fisicamente
            var existente = _pessoas.FirstOrDefault(p => p.Id == pessoaId);
            if (existente != null)
            {
                _pessoas.Remove(existente);
                Salvar();
            }
        }
    }
}
