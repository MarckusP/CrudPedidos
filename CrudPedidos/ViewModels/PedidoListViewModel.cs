using CrudPedidos.Models;
using CrudPedidos.Services;
using CrudPedidos.Views;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CrudPedidos.Views;

namespace CrudPedidos.ViewModels
{
    public class PedidoListViewModel : BaseViewModel
    {
        private readonly PedidoService _service;
        private readonly PessoaService _pessoaService;
        private readonly ProdutoService _produtoService;
        private readonly Pessoa _pessoaFiltrada; // pessoa selecionada

        public ObservableCollection<Pedido> Pedidos { get; } = new ObservableCollection<Pedido>();
        private Pedido _selected;
        public Pedido Selected { get { return _selected; } set { _selected = value; RaisePropertyChanged(); } }

        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand AtualizarCommand { get; }
        public ICommand DetalharCommand { get; }

        public PedidoListViewModel(PedidoService service, PessoaService pessoaService, ProdutoService produtoService, Pessoa pessoa = null)
        {
            _service = service;
            _pessoaService = pessoaService;
            _produtoService = produtoService;
            _pessoaFiltrada = pessoa; // guarda a pessoa filtrada

            IncluirCommand = new RelayCommand(_ => Incluir());
            EditarCommand = new RelayCommand(_ => Editar(), _ => Selected != null && Selected.Status != StatusPedido.Recebido);
            DetalharCommand = new RelayCommand(DetalharPedido, o => Selected != null);
            AtualizarCommand = new RelayCommand(_ => Carregar());

            Carregar();
        }

        private void Carregar()
        {
            Pedidos.Clear();
            var pedidos = _service.ObterTodos();

            if (_pessoaFiltrada != null)
                pedidos = pedidos.Where(p => p.PessoaId == _pessoaFiltrada.Id).ToList();

            foreach (var p in pedidos)
                Pedidos.Add(p);
        }

        private void Incluir()
        {
            var form = new PedidoFormWindow();
            var vm = new PedidoFormViewModel(_pessoaService, _produtoService, _service);

            form.DataContext = vm;

            vm.Saved += (s, e) => { form.DialogResult = true; };
            vm.Error += (s, msg) => { MessageBox.Show(msg, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning); };

            if (form.ShowDialog() == true)
                Carregar();
        }

        private StatusPedido ObterProximoStatus(StatusPedido atual)
        {
            if (atual == StatusPedido.Pendente) return StatusPedido.Pago;
            if (atual == StatusPedido.Pago) return StatusPedido.Enviado;
            if (atual == StatusPedido.Enviado) return StatusPedido.Recebido;
            return StatusPedido.Recebido;
        }

        private void Editar()
        {
            if (Selected == null || Selected.Status == StatusPedido.Recebido)
                return;

            Selected.Status = ObterProximoStatus(Selected.Status);
            _service.Atualizar(Selected);
            Carregar();
        }
        private void DetalharPedido(object obj)
        {
            if (Selected == null) return;

            var detalheVM = new PedidoDetalheViewModel(Selected);
            var detalheWindow = new PedidoDetalheWindow
            {
                DataContext = detalheVM,
                Owner = App.Current.MainWindow
            };
            detalheWindow.ShowDialog();
        }

    }
}
