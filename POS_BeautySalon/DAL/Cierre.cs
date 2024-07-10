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
    [Table("Cierres")]
    public  class Cierre
    {

        [Key]
        public int CierreId { get; set; }

        [Required]
        [DisplayName("Fecha de Cierre")]
        public DateTime FechaCierre { get; set; }

        [DisplayName("Total de Ventas de Productos")]
        public int TotalProductos { get; set; }

        [DisplayName("Total de Ventas de Servicios")]
        public int TotalServicios { get; set; }

        [Required]
        public int TotalCierre { get; set; }


    }
}
