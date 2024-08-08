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
    [Table("Comentarios")]
    public class Comentario
    {
        [Key]
        public int ComentarioId { get; set; }

        [DisplayName("Cliente")]
        public string? ClienteId { get; set; }

        [DisplayName("Comentario")]
        [Required]
        public string Detalle { get; set; }


        //Navegacion
        public ApplicationUser? Cliente { get; set; }
    }
}
