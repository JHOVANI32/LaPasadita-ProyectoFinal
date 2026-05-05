namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Inventario - Control de existencias de productos
    /// </summary>
    public class Inventario
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int CantidadDisponible { get; set; }
        public int CantidadMinima { get; set; } = 5; // Alerta de stock bajo
        public int CantidadMaxima { get; set; } = 100;
        public string? Ubicacion { get; set; } // Pasillo, estante, etc.
        public DateTime? UltimaEntrada { get; set; }
        public DateTime? UltimaSalida { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual Producto? Producto { get; set; }
    }
}
