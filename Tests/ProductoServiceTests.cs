using Microsoft.Extensions.Logging;
using Moq;
using LaPasadita.Services;
using LaPasadita.Models.DTOs;

namespace LaPasadita.Tests
{
    /// <summary>
    /// Pruebas unitarias para el servicio de productos
    /// Utilizando xUnit y Moq para simular dependencias
    /// </summary>
    public class ProductoServiceTests
    {
        private readonly Mock<ISupabaseService> _mockSupabase;
        private readonly Mock<ILogger<ProductoService>> _mockLogger;
        private readonly ProductoService _service;

        public ProductoServiceTests()
        {
            _mockSupabase = new Mock<ISupabaseService>();
            _mockLogger = new Mock<ILogger<ProductoService>>();
            _service = new ProductoService(_mockSupabase.Object, _mockLogger.Object);
        }

        #region Pruebas de Lectura (READ)

        /// <summary>
        /// Prueba: Obtener todos los productos debe retornar lista
        /// </summary>
        [Fact]
        public async Task ObtenerTodos_DebeRetornarListaDeProductos()
        {
            // Arrange
            var productosEsperados = new List<ProductoDTO>
            {
                new ProductoDTO { id = 1, nombre = "Arroz", precio = 28.50m },
                new ProductoDTO { id = 2, nombre = "Frijol", precio = 35.00m }
            };

            _mockSupabase.Setup(s => s.GetAsync<List<ProductoDTO>>(
                It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(productosEsperados);

            // Act
            var resultado = await _service.ObtenerTodosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Arroz", resultado[0].nombre);
        }

        /// <summary>
        /// Prueba: Obtener por ID existente debe retornar producto
        /// </summary>
        [Fact]
        public async Task ObtenerPorId_ConIdExistente_DebeRetornarProducto()
        {
            // Arrange
            var productoEsperado = new List<ProductoDTO>
            {
                new ProductoDTO { id = 1, nombre = "Leche", precio = 24.50m }
            };

            _mockSupabase.Setup(s => s.GetAsync<List<ProductoDTO>>(
                It.IsAny<string>(), It.Is<string>(q => q.Contains("id=eq.1"))))
                .ReturnsAsync(productoEsperado);

            // Act
            var resultado = await _service.ObtenerPorIdAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Leche", resultado.nombre);
        }

        /// <summary>
        /// Prueba: Obtener por ID inexistente debe retornar null
        /// </summary>
        [Fact]
        public async Task ObtenerPorId_ConIdInexistente_DebeRetornarNull()
        {
            // Arrange
            _mockSupabase.Setup(s => s.GetAsync<List<ProductoDTO>>(
                It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ProductoDTO>());

            // Act
            var resultado = await _service.ObtenerPorIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }

        /// <summary>
        /// Prueba: Buscar productos debe filtrar correctamente
        /// </summary>
        [Fact]
        public async Task Buscar_ConTerminoValido_DebeRetornarResultados()
        {
            // Arrange
            var productosEsperados = new List<ProductoDTO>
            {
                new ProductoDTO { id = 1, nombre = "Leche Entera", precio = 24.50m },
                new ProductoDTO { id = 2, nombre = "Leche Deslactosada", precio = 28.00m }
            };

            _mockSupabase.Setup(s => s.GetAsync<List<ProductoDTO>>(
                It.IsAny<string>(), It.Is<string>(q => q.Contains("leche"))))
                .ReturnsAsync(productosEsperados);

            // Act
            var resultado = await _service.BuscarAsync("leche");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, p => Assert.Contains("Leche", p.nombre));
        }

        #endregion

        #region Pruebas de Creación (CREATE)

        /// <summary>
        /// Prueba: Crear producto válido debe retornar producto creado
        /// </summary>
        [Fact]
        public async Task Crear_ConDatosValidos_DebeRetornarProductoCreado()
        {
            // Arrange
            var nuevoProducto = new ProductoCreateDTO
            {
                nombre = "Producto Nuevo",
                precio = 50.00m,
                categoria_id = 1
            };

            var productoCreado = new ProductoDTO
            {
                id = 10,
                nombre = "Producto Nuevo",
                precio = 50.00m,
                categoria_id = 1
            };

            _mockSupabase.Setup(s => s.PostAsync<ProductoDTO>(
                It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(productoCreado);

            // Act
            var resultado = await _service.CrearAsync(nuevoProducto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.id);
            Assert.Equal("Producto Nuevo", resultado.nombre);
        }

        #endregion

        #region Pruebas de Actualización (UPDATE)

        /// <summary>
        /// Prueba: Actualizar producto existente debe retornar producto actualizado
        /// </summary>
        [Fact]
        public async Task Actualizar_ConDatosValidos_DebeRetornarProductoActualizado()
        {
            // Arrange
            var actualizacion = new ProductoUpdateDTO
            {
                precio = 60.00m
            };

            var productoActualizado = new ProductoDTO
            {
                id = 1,
                nombre = "Producto Existente",
                precio = 60.00m
            };

            _mockSupabase.Setup(s => s.PatchAsync<ProductoDTO>(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>()))
                .ReturnsAsync(productoActualizado);

            // Act
            var resultado = await _service.ActualizarAsync(1, actualizacion);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(60.00m, resultado.precio);
        }

        #endregion

        #region Pruebas de Eliminación (DELETE)

        /// <summary>
        /// Prueba: Eliminar producto existente debe retornar true
        /// </summary>
        [Fact]
        public async Task Eliminar_ConIdExistente_DebeRetornarTrue()
        {
            // Arrange
            var productoDesactivado = new ProductoDTO { id = 1, activo = false };

            _mockSupabase.Setup(s => s.PatchAsync<ProductoDTO>(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>()))
                .ReturnsAsync(productoDesactivado);

            // Act
            var resultado = await _service.EliminarAsync(1);

            // Assert
            Assert.True(resultado);
        }

        /// <summary>
        /// Prueba: Eliminar producto inexistente debe retornar false
        /// </summary>
        [Fact]
        public async Task Eliminar_ConIdInexistente_DebeRetornarFalse()
        {
            // Arrange
            _mockSupabase.Setup(s => s.PatchAsync<ProductoDTO>(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>()))
                .ReturnsAsync((ProductoDTO?)null);

            // Act
            var resultado = await _service.EliminarAsync(999);

            // Assert
            Assert.False(resultado);
        }

        #endregion

        #region Pruebas de Categorías

        /// <summary>
        /// Prueba: Obtener categorías debe retornar lista
        /// </summary>
        [Fact]
        public async Task ObtenerCategorias_DebeRetornarListaDeCategorias()
        {
            // Arrange
            var categoriasEsperadas = new List<CategoriaDTO>
            {
                new CategoriaDTO { id = 1, nombre = "Abarrotes" },
                new CategoriaDTO { id = 2, nombre = "Lácteos" }
            };

            _mockSupabase.Setup(s => s.GetAsync<List<CategoriaDTO>>(
                It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(categoriasEsperadas);

            // Act
            var resultado = await _service.ObtenerCategoriasAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        #endregion
    }
}

/* 
============================================
INSTRUCCIONES PARA EJECUTAR LAS PRUEBAS
============================================

1. Instalar paquetes de pruebas:
   dotnet add package xunit
   dotnet add package xunit.runner.visualstudio
   dotnet add package Moq
   dotnet add package Microsoft.NET.Test.Sdk

2. Ejecutar todas las pruebas:
   dotnet test

3. Ejecutar pruebas con detalle:
   dotnet test --verbosity detailed

4. Ejecutar prueba específica:
   dotnet test --filter "ObtenerTodos_DebeRetornarListaDeProductos"

============================================
*/
