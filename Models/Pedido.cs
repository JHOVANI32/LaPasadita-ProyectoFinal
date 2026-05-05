namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Pedidos - Órdenes de compra de clientes
    /// </summary>
    public class Pedido
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty; // LP-2024-00001
        public int UsuarioId { get; set; }
        public int DireccionId { get; set; }
        public int MetodoPagoId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; } = 0;
        public decimal CostoEnvio { get; set; } = 0;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmado, EnCamino, Entregado, Cancelado
        public string? Notas { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaEntrega { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        // Navegación
        public virtual Usuario? Usuario { get; set; }
        public virtual Direccion? Direccion { get; set; }
        public virtual MetodoPago? MetodoPago { get; set; }
        public virtual ICollection<DetallesPedido>? Detalles { get; set; }
    }
}
