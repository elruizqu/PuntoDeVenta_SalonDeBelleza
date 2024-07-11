﻿using System;
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

        [ForeignKey("Servicio")]
        [DisplayName("Servicio")]
        public int? ServicioId { get; set; }

        [ForeignKey("Producto")]
        [DisplayName("Producto")]
        public int? ProductoId { get; set; }

        public DateTime Fecha { get; set; }

       

        //Navegacion
        public ApplicationUser? Cliente { get; set; }
        public Servicio? Servicio { get; set; }
        public Producto? Producto { get; set; }
    }
}
