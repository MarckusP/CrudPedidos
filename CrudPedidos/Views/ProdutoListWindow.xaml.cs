using CrudPedidos.Services;
using CrudPedidos.ViewModels;
using System;
using System.Windows;

namespace CrudPedidos.Views
{
    public partial class ProdutoListWindow : Window
    {
        public ProdutoListWindow(ProdutoService produtoService, PedidoService pedidoService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            DataContext = new ProdutoListViewModel(produtoService, pedidoService);
            
        }
    }
}
