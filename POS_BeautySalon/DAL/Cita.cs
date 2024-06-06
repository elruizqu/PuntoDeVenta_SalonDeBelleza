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
    [Table("Citas")]
    public class Cita
    {

      [Key]
        public int CitaId { get; set; }

        [DefaultValue(1)]
        public int Estado { get; set; }

        [DisplayName("Cliente")]
        public string ClienteId { get; set; }
   

        [ForeignKey("Servicio")]
        [DisplayName("Servicio")]
        public int ServicioId { get; set; }

        public DateTime Fecha { get; set; }
        public DateTime Hora { get; set; }

        //Navegacion
        public ApplicationUser? Cliente { get; set; }
        public Servicio? Servicio { get; set; }

    }
}
