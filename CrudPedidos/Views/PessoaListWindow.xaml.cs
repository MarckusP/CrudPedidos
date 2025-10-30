using CrudPedidos.Services;
using CrudPedidos.ViewModels;
using System.Windows;

namespace CrudPedidos.Views
{
    public partial class PessoaListWindow : Window
    {
        public PessoaListWindow(PessoaService pessoaService, PedidoService pedidoService, ProdutoService produtoService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            DataContext = new PessoaListViewModel(pessoaService, pedidoService, produtoService);
        }
    }
}
