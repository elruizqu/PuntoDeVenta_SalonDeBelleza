using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }

        [DisplayName("Descripción")]
        public String Descripcion { get; set; }
    }
}
