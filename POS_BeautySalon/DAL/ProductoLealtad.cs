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
    [Table("ProductosLealtad")]
    public class ProductoLealtad
    {

        [Key]
        public int ProductoLealtadId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        public String Descripcion { get; set; }

        [DisplayName("Imagen")]
        public byte[]? ImagenProductolealtad { get; set; }

        [Required]
        [DisplayName("Precio del producto en Puntos")]
        public int PrecioPuntos { get; set; }

        //Navegacion
        public ApplicationUser? Cliente { get; set; }
    }
}
