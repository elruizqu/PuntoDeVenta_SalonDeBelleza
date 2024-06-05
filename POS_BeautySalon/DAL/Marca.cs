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
    [Table("Marcas")]
    public class Marca
    {
        [Key]
        public int MarcaId { get; set; }

        [DisplayName("Descripción")]
        public String Descripcion { get; set; }
    }
}
