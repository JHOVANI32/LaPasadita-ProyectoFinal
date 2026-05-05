namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Metodos_Pago - Formas de pago disponibles
    /// </summary>
    public class MetodoPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Efectivo, Tarjeta, Transferencia
        public string? Descripcion { get; set; }
        public string? IconoUrl { get; set; }
        public bool RequiereDatosAdicionales { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<Pedido>? Pedidos { get; set; }
    }
}
