namespace LaPasadita.Models
{
    /// <summary>
    /// Modelo para la tabla Roles - Define permisos de usuario
    /// </summary>
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Admin, Cliente, Empleado
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<Usuario>? Usuarios { get; set; }
    }
}
