namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Detalle_Pedidos - Productos incluidos en cada pedido
    /// </summary>
    public class DetallesPedido
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; } = 0;
        public decimal Subtotal { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual Pedido? Pedido { get; set; }
        public virtual Producto? Producto { get; set; }
    }
}
