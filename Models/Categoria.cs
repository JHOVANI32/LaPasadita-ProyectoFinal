namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Categorías - Agrupa los productos por tipo
    /// </summary>
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<Producto>? Productos { get; set; }
    }
}
