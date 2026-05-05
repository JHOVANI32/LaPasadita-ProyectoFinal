using LaPasadita.Models.DTOs;

namespace LaPasadita.Services
{
    /// <summary>
    /// Servicio específico para operaciones con Productos
    /// Encapsula la lógica de negocio relacionada con productos
    /// </summary>
    public interface IProductoService
    {
        Task<List<ProductoDTO>> ObtenerTodosAsync();
        Task<List<ProductoDTO>> ObtenerPorCategoriaAsync(int categoriaId);
        Task<List<ProductoDTO>> BuscarAsync(string termino);
        Task<ProductoDTO?> ObtenerPorIdAsync(int id);
        Task<ProductoDTO?> CrearAsync(ProductoCreateDTO producto);
        Task<ProductoDTO?> ActualizarAsync(int id, ProductoUpdateDTO producto);
        Task<bool> EliminarAsync(int id);
        Task<List<CategoriaDTO>> ObtenerCategoriasAsync();
    }

    public class ProductoService : IProductoService
    {
        private readonly ISupabaseService _supabase;
        private readonly ILogger<ProductoService> _logger;
        private const string TABLA_PRODUCTOS = "productos";
        private const string TABLA_CATEGORIAS = "categorias";

        public ProductoService(ISupabaseService supabase, ILogger<ProductoService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los productos activos con su categoría
        /// </summary>
        public async Task<List<ProductoDTO>> ObtenerTodosAsync()
        {
            // select=*,categorias(*) hace un JOIN con la tabla categorías
            var query = "select=*,categorias(*)&activo=eq.true&order=nombre.asc";
            var productos = await _supabase.GetAsync<List<ProductoDTO>>(TABLA_PRODUCTOS, query);
            return productos ?? new List<ProductoDTO>();
        }

        /// <summary>
        /// Obtener productos filtrados por categoría
        /// </summary>
        public async Task<List<ProductoDTO>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            var query = $"select=*,categorias(*)&categoria_id=eq.{categoriaId}&activo=eq.true&order=nombre.asc";
            var productos = await _supabase.GetAsync<List<ProductoDTO>>(TABLA_PRODUCTOS, query);
            return productos ?? new List<ProductoDTO>();
        }

        /// <summary>
        /// Buscar productos por nombre o descripción
        /// </summary>
        public async Task<List<ProductoDTO>> BuscarAsync(string termino)
        {
            // ilike para búsqueda case-insensitive
            var query = $"select=*,categorias(*)&or=(nombre.ilike.%{termino}%,descripcion.ilike.%{termino}%,marca.ilike.%{termino}%)&activo=eq.true";
            var productos = await _supabase.GetAsync<List<ProductoDTO>>(TABLA_PRODUCTOS, query);
            return productos ?? new List<ProductoDTO>();
        }

        /// <summary>
        /// Obtener un producto por su ID
        /// </summary>
        public async Task<ProductoDTO?> ObtenerPorIdAsync(int id)
        {
            var query = $"select=*,categorias(*)&id=eq.{id}";
            var productos = await _supabase.GetAsync<List<ProductoDTO>>(TABLA_PRODUCTOS, query);
            return productos?.FirstOrDefault();
        }

        /// <summary>
        /// Crear un nuevo producto
        /// </summary>
        public async Task<ProductoDTO?> CrearAsync(ProductoCreateDTO producto)
        {
            _logger.LogInformation("Creando producto: {Nombre}", producto.nombre);
            return await _supabase.PostAsync<ProductoDTO>(TABLA_PRODUCTOS, producto);
        }

        /// <summary>
        /// Actualizar un producto existente
        /// </summary>
        public async Task<ProductoDTO?> ActualizarAsync(int id, ProductoUpdateDTO producto)
        {
            _logger.LogInformation("Actualizando producto ID: {Id}", id);
            return await _supabase.PatchAsync<ProductoDTO>(TABLA_PRODUCTOS, id, producto);
        }

        /// <summary>
        /// Eliminar un producto (soft delete - solo marca como inactivo)
        /// </summary>
        public async Task<bool> EliminarAsync(int id)
        {
            _logger.LogInformation("Eliminando producto ID: {Id}", id);
            // Soft delete: solo marcamos como inactivo
            var update = new { activo = false };
            var result = await _supabase.PatchAsync<ProductoDTO>(TABLA_PRODUCTOS, id, update);
            return result != null;
        }

        /// <summary>
        /// Obtener todas las categorías activas
        /// </summary>
        public async Task<List<CategoriaDTO>> ObtenerCategoriasAsync()
        {
            var query = "activo=eq.true&order=nombre.asc";
            var categorias = await _supabase.GetAsync<List<CategoriaDTO>>(TABLA_CATEGORIAS, query);
            return categorias ?? new List<CategoriaDTO>();
        }
    }
}
