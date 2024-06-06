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
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        public String Descripcion { get; set; }

        [DisplayName("Imagen")]
        public byte[]? ImagenProducto { get; set; }

        [Required]
        public int Precio { get; set; }

        [DefaultValue(1)]
        public int Estado { get; set; }

        public int Cantidad { get; set; }

        [ForeignKey("Categoria")]
        [DisplayName("Categoria")]
        public int CategoriaId { get; set; }

        [ForeignKey("Marca")]
        [DisplayName("Marca")]
        public int MarcaId { get; set; }

        [ForeignKey("Proveedor")]
        [DisplayName("Proveedor")]
        public int ProveedorId { get; set; }

        public Categoria? Categoria { get; set; }
        public Marca? Marca { get; set; }
        public Proveedor? Proveedor { get; set; }
    }
}
