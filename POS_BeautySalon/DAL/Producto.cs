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
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        public String Descripcion { get; set; }

        [DisplayName("Imagen")]
        public byte[]? ImagenProducto { get; set; }

        [Required]
        public int Precio { get; set; }

        [DisplayName("Precio de compra")]
        public int PrecioProveedor { get; set; }

        [DefaultValue(1)]
        public int Estado { get; set; }

        [DisplayName("Stock")]
        [DefaultValue(0)]
        public int Cantidad { get; set; }

        [ForeignKey("Categoria")]
        [DisplayName("Categoria")]
        public int CategoriaId { get; set; }

        [ForeignKey("Marca")]
        [DisplayName("Marca")]
        public int MarcaId { get; set; }

        [ForeignKey("Proveedor")]
        [DisplayName("Proveedor")]
        public int ProveedorId { get; set; }
        public string Alerta { get; set; }

        public Categoria? Categoria { get; set; }
        public Marca? Marca { get; set; }
        public Proveedor? Proveedor { get; set; }
        

        //Relacion Muchos a muchos con tabla intermediaria CarritoProducto
        public ICollection<CarritoProducto> CarritoProductos { get; set; } = new List<CarritoProducto>();

        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();

        public ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();

        public ICollection<ListaDeseoProducto> ListaDeseoProductos { get; set; } = new List<ListaDeseoProducto>();

        public ICollection<CarritoProvProducto> CarritoProvProductos { get; set; } = new List<CarritoProvProducto>();
    }
}
