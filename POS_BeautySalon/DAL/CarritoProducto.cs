using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    [Table("CarritoProductos")]
    public class CarritoProducto
    {
        [Key]
        public int CarritoProductoId { get; set; }

        [ForeignKey("Carrito")]
        [DisplayName("Carrito")]
        public int CarritoId { get; set; }

        [ForeignKey("Producto")]
        [DisplayName("Producto")]
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        // Propiedades de navegación
        public Carrito? Carrito { get; set; }
        public Producto? Producto { get; set; }

    }
}
