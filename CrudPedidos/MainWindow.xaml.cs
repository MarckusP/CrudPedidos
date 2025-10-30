using System.Windows;
using CrudPedidos.Services;
using CrudPedidos.Views;

namespace CrudPedidos
{
    public partial class MainWindow : Window
    {
        private readonly PessoaService _pessoaService = new PessoaService();
        private readonly ProdutoService _produtoService = new ProdutoService();
        private readonly PedidoService _pedidoService = new PedidoService();

        public MainWindow()
        {
            InitializeComponent();
            // Centraliza e abre maximizado
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
        }

        private void BtnPessoas_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // Esconde o MainWindow
            var win = new PessoaListWindow(_pessoaService, _pedidoService, _produtoService)
            {
                Owner = this
            };
            win.ShowDialog(); // Abre como modal
            this.Show(); // Volta ao MainWindow quando fechar
        }

        private void BtnProdutos_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var win = new ProdutoListWindow(_produtoService, _pedidoService)
            {
                Owner = this
            };
            win.ShowDialog();
            this.Show();
        }

        private void BtnPedidos_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var win = new PedidoListWindow(_pedidoService, _pessoaService, _produtoService)
            {
                Owner = this
            };
            win.ShowDialog();
            this.Show();
        }
    }
}
