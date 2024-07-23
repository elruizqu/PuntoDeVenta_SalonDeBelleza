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
    [Table("Facturas")]
    public class Factura
    {

        [Key]
        public int FacturaId { get; set; }

        public int PrecioTotal { get; set; }
 
        [DisplayName("Cliente")]
        public string ClienteId { get; set; } 

        public DateTime Fecha { get; set; }

       

        //Navegacion
        public ApplicationUser? Cliente { get; set; }
        public ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
