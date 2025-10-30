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
    public class PessoaPedidosViewModel : BaseViewModel
    {
        private readonly Pessoa _pessoa;
        private readonly PessoaService _pessoaService;
        private readonly ProdutoService _produtoService;
        private readonly PedidoService _pedidoService;

        private Pedido _selectedPedido;
        public ObservableCollection<Pedido> Pedidos { get; } = new ObservableCollection<Pedido>();

        public Pedido SelectedPedido
        {
            get => _selectedPedido;
            set
            {
                _selectedPedido = value;
                RaisePropertyChanged();
            }
        }

        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand AtualizarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand FecharCommand { get; }

        public PessoaPedidosViewModel(Pessoa pessoa, PessoaService pessoaService, ProdutoService produtoService, PedidoService pedidoService)
        {
            _pessoa = pessoa ?? throw new ArgumentNullException(nameof(pessoa));
            _pessoaService = pessoaService ?? throw new ArgumentNullException(nameof(pessoaService));
            _produtoService = produtoService ?? throw new ArgumentNullException(nameof(produtoService));
            _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));

            IncluirCommand = new RelayCommand(_ => Incluir());
            EditarCommand = new RelayCommand(_ => Editar(), _ => SelectedPedido != null && SelectedPedido.Status != StatusPedido.Recebido);
            AtualizarCommand = new RelayCommand(_ => CarregarPedidos(), _ => SelectedPedido != null && SelectedPedido.Status != StatusPedido.Recebido);
            ExcluirCommand = new RelayCommand(_ => Excluir(), _ => SelectedPedido != null);
            FecharCommand = new RelayCommand(win => (win as Window)?.Close());

            CarregarPedidos();
        }

        private void CarregarPedidos()
        {
            Pedidos.Clear();
            var lista = _pedidoService.ObterTodos().Where(p => p.PessoaId == _pessoa.Id);
            foreach (var p in lista)
                Pedidos.Add(p);
        }

        private void Incluir()
        {
            var form = new PedidoFormWindow();
            var vm = new PedidoFormViewModel(_pessoaService, _produtoService, _pedidoService, _pessoa);
            form.DataContext = vm;
            vm.Saved += (s, e) => form.DialogResult = true;

            if (form.ShowDialog() == true)
                CarregarPedidos();
        }

        private void Editar()
        {
            if (SelectedPedido == null) return;

            if (SelectedPedido.Status != StatusPedido.Pendente)
            {
                MessageBox.Show("Pedidos só podem ter o status alterado in-line.",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var form = new PedidoFormWindow();
            var vm = new PedidoFormViewModel(_pessoaService, _produtoService, _pedidoService, SelectedPedido);
            form.DataContext = vm;
            vm.Saved += (s, e) => form.DialogResult = true;

            if (form.ShowDialog() == true)
            {
                try
                {
                    _pedidoService.Atualizar(vm.ToModel());
                    CarregarPedidos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro ao editar pedido", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Excluir()
        {
            if (SelectedPedido == null) return;

            var result = MessageBox.Show($"Deseja realmente excluir o pedido #{SelectedPedido.Id}?",
                                         "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _pedidoService.Excluir(SelectedPedido.Id);
                CarregarPedidos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro ao excluir", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void AtualizarStatusInLine(Pedido pedido)
        {
            if (pedido == null) return;

            var proximoStatus = ObterProximoStatus(pedido.Status);
            if (pedido.Status != proximoStatus)
            {
                pedido.Status = proximoStatus;
                _pedidoService.Atualizar(pedido);
                CarregarPedidos();
            }
        }

        private StatusPedido ObterProximoStatus(StatusPedido atual)
        {
            switch (atual)
            {
                case StatusPedido.Pendente:
                    return StatusPedido.Pago;
                case StatusPedido.Pago:
                    return StatusPedido.Enviado;
                case StatusPedido.Enviado:
                    return StatusPedido.Recebido;
                default:
                    return StatusPedido.Recebido;
            }
        }
    }
}
