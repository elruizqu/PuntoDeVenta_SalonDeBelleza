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
    [Table("DetalleFacturas")]
    public class DetalleFactura

    {
        [Key]
        public int DetalleFacturaId { get; set; }

        [ForeignKey("Factura")]
        [DisplayName("Factura")]
        public int FacturaId { get; set; }

        [ForeignKey("Servicio")]
        [DisplayName("Servicio")]
        public int? ServicioId { get; set; }

        [ForeignKey("Producto")]
        [DisplayName("Producto")]
        public int? ProductoId { get; set; }

        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }



        //Navegacion
        public Servicio? Servicio { get; set; }
        public Producto? Producto { get; set; }
        public Factura? Factura { get; set; }
    }
}

