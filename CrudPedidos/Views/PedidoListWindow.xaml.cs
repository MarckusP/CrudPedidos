using CrudPedidos.Services;
using CrudPedidos.ViewModels;
using System.Windows;

namespace CrudPedidos.Views
{
    public partial class PedidoListWindow : Window
    {
        public PedidoListWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
        }

        public PedidoListWindow(PedidoService pedidoService, PessoaService pessoaService, ProdutoService produtoService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            DataContext = new PedidoListViewModel(pedidoService, pessoaService, produtoService);
        }

    }
}
