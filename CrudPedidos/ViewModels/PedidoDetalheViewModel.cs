using CrudPedidos.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CrudPedidos.ViewModels
{
    public class PedidoDetalheViewModel : BaseViewModel
    {
        public int PedidoId { get; }
        public string Cliente { get; }

        public ObservableCollection<PedidoItem> Itens { get; } = new ObservableCollection<PedidoItem>();

        public ICommand FecharCommand { get; }

        public PedidoDetalheViewModel(Pedido pedido)
        {
            PedidoId = pedido.Id;
            Cliente = pedido.PessoaNome;

            foreach (var item in pedido.Itens)
                Itens.Add(item);

            FecharCommand = new RelayCommand(o => ((System.Windows.Window)o).Close());
        }
    }
}
