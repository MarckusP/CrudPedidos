using CrudPedidos.Models;
using CrudPedidos.Services;
using CrudPedidos.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CrudPedidos.ViewModels
{
    public class ProdutoListViewModel : BaseViewModel
    {
        private readonly ProdutoService _produtoService;
        private readonly PedidoService _pedidoService;
        private Produto _selected;

        private string _filtroNome;
        private string _filtroCodigo;
        private string _filtroValorMin;
        private string _filtroValorMax;
        private string _filtroStatus = "Ambos";

        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();

        public Produto Selected
        {
            get => _selected;
            set { _selected = value; RaisePropertyChanged(); }
        }

        public string FiltroNome { get => _filtroNome; set { _filtroNome = value; RaisePropertyChanged(); } }
        public string FiltroCodigo { get => _filtroCodigo; set { _filtroCodigo = value; RaisePropertyChanged(); } }
        public string FiltroValorMin { get => _filtroValorMin; set { _filtroValorMin = value; RaisePropertyChanged(); } }
        public string FiltroValorMax { get => _filtroValorMax; set { _filtroValorMax = value; RaisePropertyChanged(); } }
        public string FiltroStatus
        {
            get => _filtroStatus;
            set
            {
                if (_filtroStatus == value) return;
                _filtroStatus = value;
                RaisePropertyChanged();
                CarregarFiltrado();
            }
        }

        public ICommand FiltrarCommand { get; }
        public ICommand LimparCommand { get; }
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand AlternarAtivoCommand { get; }
        public ICommand ExcluirCommand { get; }

        public ProdutoListViewModel(ProdutoService produtoService, PedidoService pedidoService)
        {
            _produtoService = produtoService ?? throw new ArgumentNullException(nameof(produtoService));
            _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));

            FiltrarCommand = new RelayCommand(_ => CarregarFiltrado());
            LimparCommand = new RelayCommand(_ =>
            {
                FiltroNome = FiltroCodigo = FiltroValorMin = FiltroValorMax = string.Empty;
                CarregarTodos();
            });
            IncluirCommand = new RelayCommand(_ => Incluir());
            EditarCommand = new RelayCommand(_ => Editar(), _ => Selected != null);
            AlternarAtivoCommand = new RelayCommand(_ => AlternarAtivo(), _ => Selected != null);
            ExcluirCommand = new RelayCommand(_ => ExcluirProduto(), _ => Selected != null);

            CarregarTodos();
        }

        private void CarregarTodos()
        {
            AtualizarLista(_produtoService.ObterTodos());
        }

        private void CarregarFiltrado()
        {
            decimal? vmin = null, vmax = null;
            if (Produto.TryParseValor(FiltroValorMin, out var vm)) vmin = vm;
            if (Produto.TryParseValor(FiltroValorMax, out var vx)) vmax = vx;

            var lista = _produtoService.Filtrar(FiltroNome, FiltroCodigo, vmin, vmax, incluirInativos: true, status: FiltroStatus);
            AtualizarLista(lista);
        }

        private void AtualizarLista(System.Collections.Generic.IEnumerable<Produto> lista)
        {
            Produtos.Clear();
            foreach (var p in lista)
                Produtos.Add(p);
        }

        private void Incluir()
        {
            var form = new ProdutoFormWindow();
            var vm = new ProdutoFormViewModel();
            form.DataContext = vm;
            vm.Saved += (s, e) => { form.DialogResult = true; };
            if (form.ShowDialog() == true)
            {
                try
                {
                    _produtoService.Adicionar(vm.Nome, vm.Codigo, vm.Valor);
                    CarregarFiltrado();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Editar()
        {
            if (Selected == null) return;
            var form = new ProdutoFormWindow();
            var vm = new ProdutoFormViewModel(Selected);
            form.DataContext = vm;
            vm.Saved += (s, e) => { form.DialogResult = true; };
            if (form.ShowDialog() == true)
            {
                try
                {
                    _produtoService.Atualizar(vm.ToModel());
                    CarregarFiltrado();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void AlternarAtivo()
        {
            if (Selected == null) return;
            string novoStatus = Selected.Status == "A" ? "I" : "A";
            _produtoService.AlterarStatus(Selected.Id, novoStatus);
            CarregarFiltrado();
        }



        private void ExcluirProduto()
        {
            if (Selected == null) return;

            var result = MessageBox.Show(
                $"Deseja realmente excluir o produto '{Selected.Nome}'?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var pedidos = _pedidoService.ObterTodos();

                if (pedidos.Any(p => p.Itens.Any(i => i.ProdutoId == Selected.Id)))
                    throw new InvalidOperationException(
                        "Não é possível excluir este produto. Existem pedidos associados."
                    );

                _produtoService.Excluir(Selected.Id);
                CarregarFiltrado();

                MessageBox.Show(
                    "Produto excluído com sucesso.",
                    "Sucesso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Erro ao excluir",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
    }
}
