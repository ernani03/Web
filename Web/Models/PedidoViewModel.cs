using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class PedidoViewModel
    {
        public int IdCliente { get; set; }
        public int Total { get; set; }
        public List<DetalleItemViewModel> Detalles { get; set; }
    }

    public class DetalleItemViewModel
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public int Precio { get; set; }
    }
}