using CrudPedidos.Models;
using CrudPedidos.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CrudPedidos.ViewModels
{
    public class PedidoFormViewModel : BaseViewModel
    {
        private readonly PessoaService _pessoaService;
        private readonly ProdutoService _produtoService;
        private readonly PedidoService _pedidoService;

        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();
        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();
        public ObservableCollection<PedidoItem> Itens { get; } = new ObservableCollection<PedidoItem>();

        private Pessoa _pessoaSelecionada;
        public Pessoa PessoaSelecionada { get => _pessoaSelecionada; set { _pessoaSelecionada = value; RaisePropertyChanged(); } }

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado { get => _produtoSelecionado; set { _produtoSelecionado = value; RaisePropertyChanged(); } }

        private int _quantidade = 1;
        public int Quantidade { get => _quantidade; set { _quantidade = value; RaisePropertyChanged(); } }

        private FormaPagamento _formaPagamento = FormaPagamento.Dinheiro;
        public FormaPagamento FormaPagamento { get => _formaPagamento; set { _formaPagamento = value; RaisePropertyChanged(); } }

        private StatusPedido _status;
        public StatusPedido Status { get => _status; set { _status = value; RaisePropertyChanged(); } }

        public decimal ValorTotal => Itens.Sum(i => i.ValorTotal);

        public ICommand AdicionarItemCommand { get; }
        public ICommand RemoverItemCommand { get; }
        public ICommand FinalizarCommand { get; }

        public event EventHandler Saved;
        public event EventHandler<string> Error;

        private bool _isEditing;
        private Pedido _editingPedido;

        // Construtor para inclusão
        public PedidoFormViewModel(PessoaService pessoaService, ProdutoService produtoService, PedidoService pedidoService, Pessoa pessoa = null)
        {
            _pessoaService = pessoaService;
            _produtoService = produtoService;
            _pedidoService = pedidoService;

            CarregarCatalogos();
            AdicionarItemCommand = new RelayCommand(_ => AdicionarItem());
            RemoverItemCommand = new RelayCommand(param => RemoverItem(param as PedidoItem));
            FinalizarCommand = new RelayCommand(_ => Finalizar());

            if (pessoa != null)
                PessoaSelecionada = pessoa;

            Itens.CollectionChanged += (s, e) => RaisePropertyChanged(nameof(ValorTotal));
        }

        // Construtor para edição
        public PedidoFormViewModel(PessoaService pessoaService, ProdutoService produtoService, PedidoService pedidoService, Pedido pedido)
            : this(pessoaService, produtoService, pedidoService)
        {
            if (pedido == null) throw new ArgumentNullException(nameof(pedido));

            _isEditing = true;
            _editingPedido = pedido;

            PessoaSelecionada = Pessoas.FirstOrDefault(p => p.Id == pedido.PessoaId);
            FormaPagamento = pedido.FormaPagamento;
            Status = pedido.Status;

            Itens.Clear();
            foreach (var i in pedido.Itens)
                Itens.Add(new PedidoItem
                {
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.ProdutoNome,
                    ProdutoCodigo = i.ProdutoCodigo,
                    ValorUnitario = i.ValorUnitario,
                    Quantidade = i.Quantidade
                });
        }

        private void CarregarCatalogos()
        {
            Pessoas.Clear();
            foreach (var p in _pessoaService.Filtrar(null, null))
                Pessoas.Add(p);

            Produtos.Clear();
            foreach (var p in _produtoService.Filtrar(null, null, null, null))
                Produtos.Add(p);
        }

        private void AdicionarItem()
        {
            if (ProdutoSelecionado == null) { Error?.Invoke(this, "Selecione um produto."); return; }
            if (Quantidade <= 0) { Error?.Invoke(this, "Quantidade deve ser maior que zero."); return; }

            var existente = Itens.FirstOrDefault(i => i.ProdutoId == ProdutoSelecionado.Id);
            if (existente != null) existente.Quantidade += Quantidade;
            else
                Itens.Add(new PedidoItem
                {
                    ProdutoId = ProdutoSelecionado.Id,
                    ProdutoNome = ProdutoSelecionado.Nome,
                    ProdutoCodigo = ProdutoSelecionado.Codigo,
                    ValorUnitario = ProdutoSelecionado.Valor,
                    Quantidade = Quantidade
                });

            RaisePropertyChanged(nameof(ValorTotal));
        }

        private void RemoverItem(PedidoItem item)
        {
            if (item == null) return;
            Itens.Remove(item);
            RaisePropertyChanged(nameof(ValorTotal));
        }

        private void Finalizar()
        {
            try
            {
                if (PessoaSelecionada == null) throw new ArgumentException("Selecione uma pessoa.");

                if (_isEditing)
                {
                    // Atualizar pedido existente
                    _editingPedido.PessoaId = PessoaSelecionada.Id;
                    _editingPedido.PessoaNome = PessoaSelecionada.Nome;
                    _editingPedido.FormaPagamento = FormaPagamento;
                    _editingPedido.Status = Status;
                    _editingPedido.Itens = Itens.Select(i => new PedidoItem
                    {
                        ProdutoId = i.ProdutoId,
                        ProdutoNome = i.ProdutoNome,
                        ProdutoCodigo = i.ProdutoCodigo,
                        ValorUnitario = i.ValorUnitario,
                        Quantidade = i.Quantidade
                    }).ToList();

                    _pedidoService.Atualizar(_editingPedido);
                }
                else
                {
                    // Novo pedido
                    var pedido = new Pedido
                    {
                        PessoaId = PessoaSelecionada.Id,
                        PessoaNome = PessoaSelecionada.Nome,
                        FormaPagamento = FormaPagamento,
                        Status = Status,
                        Itens = Itens.Select(i => new PedidoItem
                        {
                            ProdutoId = i.ProdutoId,
                            ProdutoNome = i.ProdutoNome,
                            ProdutoCodigo = i.ProdutoCodigo,
                            ValorUnitario = i.ValorUnitario,
                            Quantidade = i.Quantidade
                        }).ToList()
                    };
                    _pedidoService.Adicionar(pedido);
                }

                Saved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, ex.Message);
            }
        }
        public Pedido ToModel()
        {
            if (PessoaSelecionada == null)
                throw new InvalidOperationException("Selecione uma pessoa.");

            return new Pedido
            {
                PessoaId = PessoaSelecionada.Id,
                PessoaNome = PessoaSelecionada.Nome,
                FormaPagamento = this.FormaPagamento,
                Status = this.Status,
                Itens = this.Itens.Select(i => new PedidoItem
                {
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.ProdutoNome,
                    ProdutoCodigo = i.ProdutoCodigo,
                    ValorUnitario = i.ValorUnitario,
                    Quantidade = i.Quantidade
                }).ToList()
            };
        }
    }
}
