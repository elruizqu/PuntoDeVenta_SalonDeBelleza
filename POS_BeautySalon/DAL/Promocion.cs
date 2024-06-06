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
    [Table("Promociones")]
    public class Promocion
    {

        [Key]
        public int PromocionId { get; set; }

        [DisplayName("Descripción")]
        public String Descripcion { get; set; }

        [DisplayName("Imagen")]
        public byte[]? ImagenPromocion { get; set; }


        [ForeignKey("Servicio")]
        [DisplayName("Servicio")]
        public int? ServicioId { get; set; }

        [ForeignKey("Producto")]
        [DisplayName("Producto")]
        public int? ProductoId { get; set; }


       
        


    }
}
