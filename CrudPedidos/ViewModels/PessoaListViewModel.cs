using CrudPedidos.Models;
using CrudPedidos.Services;
using CrudPedidos.Views;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CrudPedidos.ViewModels
{
    public class PessoaListViewModel : BaseViewModel
    {
        private readonly PessoaService _service;
        private readonly ProdutoService _produtoService;
        private readonly PedidoService _pedidoService;

        private string _filtroNome;
        private string _filtroCpf;
        private string _filtroStatus = "Ambos";
        private Pessoa _selectedPessoa;

        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();

        public string FiltroNome
        {
            get => _filtroNome;
            set { _filtroNome = value; RaisePropertyChanged(); }
        }

        public string FiltroCpf
        {
            get => _filtroCpf;
            set { _filtroCpf = value; RaisePropertyChanged(); }
        }

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

        public Pessoa SelectedPessoa
        {
            get => _selectedPessoa;
            set { _selectedPessoa = value; RaisePropertyChanged(); }
        }

        public ICommand FiltrarCommand { get; }
        public ICommand LimparCommand { get; }
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand AlternarAtivoCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand AbrirPedidosCommand { get; }


        // Construtor agora recebe o PedidoService
        public PessoaListViewModel(PessoaService service, PedidoService pedidoService, ProdutoService produtoService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _produtoService = produtoService ?? throw new ArgumentNullException(nameof(produtoService));
            _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));

            FiltrarCommand = new RelayCommand(_ => CarregarFiltrado());
            LimparCommand = new RelayCommand(_ => { FiltroNome = string.Empty; FiltroCpf = string.Empty; CarregarTodos(); });
            IncluirCommand = new RelayCommand(_ => Incluir());
            EditarCommand = new RelayCommand(_ => Editar(), _ => SelectedPessoa != null);
            AlternarAtivoCommand = new RelayCommand(_ => AlternarAtivo(), _ => SelectedPessoa != null);
            ExcluirCommand = new RelayCommand(_ => Excluir(), _ => SelectedPessoa != null);
            AbrirPedidosCommand = new RelayCommand(_ => AbrirPedidos(), _ => _service != null);


            CarregarTodos();
        }

        private void CarregarTodos() => AtualizarLista(_service.ObterTodos());

        private void CarregarFiltrado() =>
            AtualizarLista(_service.Filtrar(FiltroNome, FiltroCpf, incluirInativos: true, status: FiltroStatus));

        private void AtualizarLista(System.Collections.Generic.IEnumerable<Pessoa> lista)
        {
            Pessoas.Clear();
            foreach (var p in lista)
                Pessoas.Add(p);
        }

        private void Incluir()
        {
            var form = new PessoaFormWindow();
            var vm = new PessoaFormViewModel();
            form.DataContext = vm;
            vm.Saved += (s, e) => form.DialogResult = true;

            if (form.ShowDialog() == true)
            {
                try
                {
                    _service.Adicionar(vm.Nome, vm.Cpf);
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
            if (SelectedPessoa == null) return;

            var form = new PessoaFormWindow();
            var vm = new PessoaFormViewModel(SelectedPessoa);
            form.DataContext = vm;
            vm.Saved += (s, e) => form.DialogResult = true;

            if (form.ShowDialog() == true)
            {
                try
                {
                    _service.Atualizar(vm.ToModel());
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
            if (SelectedPessoa == null) return;
            string novoStatus = SelectedPessoa.Status == "A" ? "I" : "A";
            _service.AlterarStatus(SelectedPessoa.Id, novoStatus);
            CarregarFiltrado();
        }

        private void Excluir()
        {
            if (SelectedPessoa == null) return;

            var result = MessageBox.Show($"Deseja realmente excluir a pessoa '{SelectedPessoa.Nome}'?",
                                         "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var pedidos = _pedidoService.ObterTodos();

                if (pedidos.Any(p => p.PessoaId == SelectedPessoa.Id))
                    throw new InvalidOperationException("Não é possível excluir esta pessoa, pois há pedidos associados a ela.");

                _service.Excluir(SelectedPessoa.Id, pedidos);
                CarregarFiltrado();

                MessageBox.Show("Pessoa excluída com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro ao excluir", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AbrirPedidos()
        {
            if (SelectedPessoa == null)
            {
                MessageBox.Show("Selecione uma pessoa para visualizar os pedidos.",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Cria a janela de lista de pedidos
            var janela = new PedidoListWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new PedidoListViewModel(
                    _pedidoService,   // PedidoService
                    _service,         // PessoaService
                    _produtoService,  // ProdutoService
                    SelectedPessoa    // Pessoa filtrada
                )
            };

            janela.ShowDialog();
        }

    }

}


