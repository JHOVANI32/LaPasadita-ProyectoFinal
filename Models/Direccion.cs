namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Direcciones - Direcciones de entrega de usuarios
    /// </summary>
    public class Direccion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Calle { get; set; } = string.Empty;
        public string NumeroExterior { get; set; } = string.Empty;
        public string? NumeroInterior { get; set; }
        public string Colonia { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Referencias { get; set; }
        public bool EsPrincipal { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<Pedido>? Pedidos { get; set; }
    }
}
