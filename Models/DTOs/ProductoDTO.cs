namespace LaPasadita.Models.DTOs
{
    /// <summary>
    /// Data Transfer Objects para comunicación con Supabase
    /// </summary>
    public class ProductoDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string? descripcion { get; set; }
        public decimal precio { get; set; }
        public decimal? precio_oferta { get; set; }
        public string? imagen_url { get; set; }
        public string? codigo_barras { get; set; }
        public string? marca { get; set; }
        public string? unidad_medida { get; set; }
        public int categoria_id { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
        
        // Para join con categorías
        public CategoriaDTO? categorias { get; set; }
    }

    public class CategoriaDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string? descripcion { get; set; }
        public string? imagen_url { get; set; }
        public bool activo { get; set; }
    }

    public class ProductoCreateDTO
    {
        public string nombre { get; set; } = string.Empty;
        public string? descripcion { get; set; }
        public decimal precio { get; set; }
        public decimal? precio_oferta { get; set; }
        public string? imagen_url { get; set; }
        public string? codigo_barras { get; set; }
        public string? marca { get; set; }
        public string? unidad_medida { get; set; }
        public int categoria_id { get; set; }
        public bool activo { get; set; } = true;
    }

    public class ProductoUpdateDTO
    {
        public string? nombre { get; set; }
        public string? descripcion { get; set; }
        public decimal? precio { get; set; }
        public decimal? precio_oferta { get; set; }
        public string? imagen_url { get; set; }
        public string? codigo_barras { get; set; }
        public string? marca { get; set; }
        public string? unidad_medida { get; set; }
        public int? categoria_id { get; set; }
        public bool? activo { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}
