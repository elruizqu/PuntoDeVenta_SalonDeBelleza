using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    [Table("DetalleProveedorFacturas")]
    public class DetalleProveedorFactura
    {
        [Key]
        public int DetalleProveedorFacturaId { get; set; }

        [ForeignKey("FacturaProveedor")]
        [DisplayName("Factura")]
        public int FacturaProveedorId { get; set; }

        [ForeignKey("Producto")]
        [DisplayName("Producto")]
        public int? ProductoId { get; set; }

        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }


        //Navegacion
        public Producto? Producto { get; set; }
        public Factura? Factura { get; set; }
    }
}
