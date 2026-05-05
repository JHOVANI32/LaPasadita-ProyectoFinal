namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Promociones - Ofertas y descuentos especiales
    /// </summary>
    public class Promocion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string TipoDescuento { get; set; } = "Porcentaje"; // Porcentaje, MontoFijo
        public decimal ValorDescuento { get; set; }
        public int? ProductoId { get; set; } // null = aplica a toda la tienda
        public int? CategoriaId { get; set; } // null = aplica a productos específicos
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual Producto? Producto { get; set; }
        public virtual Categoria? Categoria { get; set; }
    }
}
