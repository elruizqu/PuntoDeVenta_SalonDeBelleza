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
    [Table("CarritoProvProductos")]
    public class CarritoProvProducto
    {
        [Key]
        public int CarritoProvProductoId { get; set; }

        [ForeignKey("CarritoProveedor")]
        [DisplayName("Carrito")]
        public int CarritoProveedorId { get; set; }

        [ForeignKey("Producto")]
        [DisplayName("Producto")]
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        // Propiedades de navegación
        public CarritoProveedor? CarritoProveedor { get; set; }
        public Producto? Producto { get; set; }
    }
}
