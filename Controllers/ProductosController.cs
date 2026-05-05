using Microsoft.AspNetCore.Mvc;
using LaPasadita.Models.DTOs;
using LaPasadita.Services;

namespace LaPasadita.Controllers
{
    /// <summary>
    /// Controlador de Productos - Maneja el CRUD completo
    /// Incluye endpoints para la vista MVC y API para AJAX
    /// </summary>
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        #region Vistas MVC

        /// <summary>
        /// Vista principal del catálogo de productos
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Vista para crear nuevo producto (Admin)
        /// </summary>
        public async Task<IActionResult> Crear()
        {
            ViewBag.Categorias = await _productoService.ObtenerCategoriasAsync();
            return View();
        }

        /// <summary>
        /// Vista para editar producto (Admin)
        /// </summary>
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewBag.Categorias = await _productoService.ObtenerCategoriasAsync();
            return View(producto);
        }

        #endregion

        #region API Endpoints para AJAX

        /// <summary>
        /// GET: api/productos - Obtener todos los productos
        /// </summary>
        [HttpGet]
        [Route("api/productos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var productos = await _productoService.ObtenerTodosAsync();
                return Json(new ApiResponse<List<ProductoDTO>>
                {
                    Success = true,
                    Data = productos,
                    Message = $"Se encontraron {productos.Count} productos"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al obtener productos",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// GET: api/productos/categoria/{id} - Filtrar por categoría
        /// </summary>
        [HttpGet]
        [Route("api/productos/categoria/{categoriaId}")]
        public async Task<IActionResult> ObtenerPorCategoria(int categoriaId)
        {
            try
            {
                var productos = await _productoService.ObtenerPorCategoriaAsync(categoriaId);
                return Json(new ApiResponse<List<ProductoDTO>>
                {
                    Success = true,
                    Data = productos,
                    Message = $"Se encontraron {productos.Count} productos en la categoría"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar productos por categoría");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al filtrar productos"
                });
            }
        }

        /// <summary>
        /// GET: api/productos/buscar?q={termino} - Buscar productos
        /// </summary>
        [HttpGet]
        [Route("api/productos/buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    var todos = await _productoService.ObtenerTodosAsync();
                    return Json(new ApiResponse<List<ProductoDTO>>
                    {
                        Success = true,
                        Data = todos
                    });
                }

                var productos = await _productoService.BuscarAsync(q);
                return Json(new ApiResponse<List<ProductoDTO>>
                {
                    Success = true,
                    Data = productos,
                    Message = $"Se encontraron {productos.Count} resultados para '{q}'"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al buscar productos"
                });
            }
        }

        /// <summary>
        /// GET: api/productos/{id} - Obtener un producto
        /// </summary>
        [HttpGet]
        [Route("api/productos/{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);
                if (producto == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Producto no encontrado"
                    });
                }

                return Json(new ApiResponse<ProductoDTO>
                {
                    Success = true,
                    Data = producto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al obtener el producto"
                });
            }
        }

        /// <summary>
        /// POST: api/productos - Crear nuevo producto
        /// </summary>
        [HttpPost]
        [Route("api/productos")]
        public async Task<IActionResult> Crear([FromBody] ProductoCreateDTO producto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var nuevoProducto = await _productoService.CrearAsync(producto);
                if (nuevoProducto == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "No se pudo crear el producto"
                    });
                }

                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoProducto.id },
                    new ApiResponse<ProductoDTO>
                    {
                        Success = true,
                        Data = nuevoProducto,
                        Message = "Producto creado exitosamente"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al crear el producto"
                });
            }
        }

        /// <summary>
        /// PUT: api/productos/{id} - Actualizar producto
        /// </summary>
        [HttpPut]
        [Route("api/productos/{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ProductoUpdateDTO producto)
        {
            try
            {
                var productoActualizado = await _productoService.ActualizarAsync(id, producto);
                if (productoActualizado == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Producto no encontrado"
                    });
                }

                return Json(new ApiResponse<ProductoDTO>
                {
                    Success = true,
                    Data = productoActualizado,
                    Message = "Producto actualizado exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al actualizar el producto"
                });
            }
        }

        /// <summary>
        /// DELETE: api/productos/{id} - Eliminar producto
        /// </summary>
        [HttpDelete]
        [Route("api/productos/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _productoService.EliminarAsync(id);
                if (!resultado)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Producto no encontrado o ya fue eliminado"
                    });
                }

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Producto eliminado exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al eliminar el producto"
                });
            }
        }

        /// <summary>
        /// GET: api/categorias - Obtener todas las categorías
        /// </summary>
        [HttpGet]
        [Route("api/categorias")]
        public async Task<IActionResult> ObtenerCategorias()
        {
            try
            {
                var categorias = await _productoService.ObtenerCategoriasAsync();
                return Json(new ApiResponse<List<CategoriaDTO>>
                {
                    Success = true,
                    Data = categorias
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al obtener categorías"
                });
            }
        }

        #endregion
    }
}
