using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CrudPedidos.Models
{
    public enum FormaPagamento
    {
        Dinheiro,
        Cartao,
        Boleto
    }

    public enum StatusPedido
    {
        Pendente,
        Pago,
        Enviado,
        Recebido
    }

    public class PedidoItem
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public string ProdutoCodigo { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorTotal => ValorUnitario * Quantidade;
    }

    public class Pedido : INotifyPropertyChanged
    {

        public int Id { get; set; }
        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }
        public DateTime DataVenda { get; set; }
        public FormaPagamento FormaPagamento { get; set; }

        private StatusPedido _status;
        public StatusPedido Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public List<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
        public decimal ValorTotal => Itens.Sum(i => i.ValorTotal);

        public Pedido()
        {
            Status = StatusPedido.Pendente; // inicializa como Pendente
        }

        public void Validar()
        {
            if (PessoaId <= 0)
                throw new ArgumentException("Pessoa é obrigatória.");
            if (Itens == null || Itens.Count == 0)
                throw new ArgumentException("Informe ao menos um produto.");
            if (Itens.Any(i => i.Quantidade <= 0))
                throw new ArgumentException("Quantidade deve ser maior que zero.");
            if (ValorTotal <= 0)
                throw new ArgumentException("Valor total inválido.");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public IEnumerable<StatusPedido> StatusDisponiveis
        {
            get
            {
                switch (Status)
                {
                    case StatusPedido.Pendente:
                        return new[] { StatusPedido.Pendente, StatusPedido.Pago };
                    case StatusPedido.Pago:
                        return new[] { StatusPedido.Pago, StatusPedido.Enviado };
                    case StatusPedido.Enviado:
                        return new[] { StatusPedido.Enviado, StatusPedido.Recebido };
                    case StatusPedido.Recebido:
                        return new[] { StatusPedido.Recebido };
                    default:
                        return new[] { StatusPedido.Pendente };
                }
            }
        }
    }
}
