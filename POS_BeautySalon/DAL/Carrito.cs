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
    [Table("Carritos")]
    public class Carrito
    {

        [Key]
        public int CarritoId { get; set; }

        [DisplayName("Cliente")]
        public string ClienteId { get; set; }


        public ApplicationUser? Cliente { get; set; }


        //Relacion Muchos a muchos con tabla intermediaria CarritoProducto
        public ICollection<CarritoProducto> CarritoProductos { get; set; } = new List<CarritoProducto>();

        //Método para calcular el monto total del carrito
        public int CalcularTotal()
        {
            int total = 0;

            foreach (var carritoProducto in CarritoProductos)
            {
                total += carritoProducto.Producto.Precio * carritoProducto.Cantidad;
            }

            return total;
        }
    }
}
