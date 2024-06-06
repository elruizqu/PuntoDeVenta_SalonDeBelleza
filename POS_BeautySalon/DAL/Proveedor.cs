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
    [Table("Proveedores")]
    public class Proveedor
    {

        [Key]
        public int ProveedorId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Email { get; set; }

        [DisplayName("Teléfono")]
        public String Telefono { get; set; }

       
        public int Puntualidad { get; set; }
        public int Calidad { get; set; }

        [DisplayName("Calificación")] //calificacion general promedio entre puntualidad y calidad para reportes
        public int Calificacion { get; set; }


        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
