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
    [Table("ListaDeseoProductos")]
    public class ListaDeseoProducto
    {
        [Key]
        public int ListaDeseoProductoId { get; set; }

        [ForeignKey("ListaDeseo")]
        [DisplayName("Lista de deseos")]
        public int ListaID { get; set; }

        [ForeignKey("Producto")]
        [DisplayName("Producto")]
        public int ProductoId { get; set; }

        // Propiedades de navegación
        public ListaDeseo? ListaDeseo { get; set; }
        public Producto? Producto { get; set; }
    }
}
