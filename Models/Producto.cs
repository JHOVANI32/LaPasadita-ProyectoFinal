namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Productos - Artículos disponibles en la tienda
    /// </summary>
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal? PrecioOferta { get; set; }
        public string? ImagenUrl { get; set; }
        public string? CodigoBarras { get; set; }
        public string? Marca { get; set; }
        public string? UnidadMedida { get; set; } // kg, litro, pieza, etc.
        public int CategoriaId { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }

        // Navegación
        public virtual Categoria? Categoria { get; set; }
        public virtual Inventario? Inventario { get; set; }
        public virtual ICollection<DetallesPedido>? DetallesPedidos { get; set; }
        public virtual ICollection<Promocion>? Promociones { get; set; }
    }
}
