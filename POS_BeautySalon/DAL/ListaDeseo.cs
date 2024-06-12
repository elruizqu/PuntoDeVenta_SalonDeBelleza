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
    [Table("ListaDeseos")]
    public class ListaDeseo
    {

        [Key]
        public int ListaID { get; set; }

        [DisplayName("Cliente")]
        public string ClienteId { get; set; }


        public ApplicationUser? Cliente { get; set; }

        public ICollection<ListaDeseoProducto> ListaDeseoProductos { get; set; } = new List<ListaDeseoProducto>();

    }
}
