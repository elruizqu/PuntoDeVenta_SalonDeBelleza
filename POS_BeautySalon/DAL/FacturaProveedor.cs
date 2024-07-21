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
    [Table("FacturaProveedores")]
    public class FacturaProveedor
    {
        [Key]
        public int FacturaProveedorId { get; set; }

        public int PrecioTotal { get; set; }

        [ForeignKey("Proveedor")]
        [DisplayName("Proveedor")]
        public int ProveedorId { get; set; }

        public DateTime Fecha { get; set; }

        //Navegacion
        public Proveedor? Proveedor { get; set; }
        public ICollection<DetalleProveedorFactura> DetalleProveedorFacturas { get; set; } = new List<DetalleProveedorFactura>();
    }
}
