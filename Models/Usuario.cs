namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Usuarios - Clientes y administradores del sistema
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public int RolId { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? UltimoAcceso { get; set; }

        // Navegación
        public virtual Rol? Rol { get; set; }
        public virtual ICollection<Direccion>? Direcciones { get; set; }
        public virtual ICollection<Pedido>? Pedidos { get; set; }
    }
}
