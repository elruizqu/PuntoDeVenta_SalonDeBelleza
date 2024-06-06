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
    [Table("Servicios")]
    public class Servicio
    {

        [Key]
        public int ServicioId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        public String Descripcion { get; set; }

        [DisplayName("Imagen")]
        public byte[]? ImagenServicio { get; set; }
        [Required]
        public int Precio { get; set; }

        public ICollection<Cita> Cita { get; set; } = new List<Cita>();
        public Proveedor? Proveedor { get; set; }

        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();

        public ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();
    }
}
