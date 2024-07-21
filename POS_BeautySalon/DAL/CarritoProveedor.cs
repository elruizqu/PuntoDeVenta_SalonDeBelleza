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
    [Table("CarritoProveedores")]
    public class CarritoProveedor
    {
        [Key]
        public int CarritoProveedorId { get; set; }

        [DisplayName("Proveedor")]
        public int ProveedorId { get; set; }

        public Proveedor? Proveedor { get; set; }

        //Relacion Muchos a muchos con tabla intermediaria CarritoProducto
        public ICollection<CarritoProvProducto> CarritoProvProductos { get; set; } = new List<CarritoProvProducto>();

        //Método para calcular el monto total del carrito
        public int CalcularTotal()
        {
            int total = 0;

            foreach (var carritoProvProducto in CarritoProvProductos)
            {
                total += carritoProvProducto.Producto.PrecioProveedor * carritoProvProducto.Cantidad;
            }

            return total;
        }
    }
}
