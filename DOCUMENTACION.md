# La Pasadita - Documentación del Proyecto

## Descripción General
Sistema de tienda de abarrotes desarrollado en ASP.NET Core MVC 8.0 con Supabase como backend de base de datos.

---

## Arquitectura del Sistema

```
┌─────────────────────────────────────────────────────────────────┐
│                         FRONTEND                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │   Vista Razor (.cshtml) + Tailwind CSS + JavaScript      │   │
│  │   - Catálogo de productos dinámico                       │   │
│  │   - Llamadas AJAX para evitar recargas                   │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼ HTTP/AJAX
┌─────────────────────────────────────────────────────────────────┐
│                         BACKEND                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │   Controlador (ProductosController.cs)                   │   │
│  │   - Endpoints API REST (/api/productos)                  │   │
│  │   - Vistas MVC tradicionales                             │   │
│  └─────────────────────────────────────────────────────────┘   │
│                              │                                   │
│                              ▼                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │   Servicio de Productos (ProductoService.cs)             │   │
│  │   - Lógica de negocio                                    │   │
│  │   - Validaciones                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                              │                                   │
│                              ▼                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │   Servicio Supabase (SupabaseService.cs)                 │   │
│  │   - Conexión HTTP a la API REST de Supabase              │   │
│  │   - Operaciones CRUD genéricas                           │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼ HTTPS
┌─────────────────────────────────────────────────────────────────┐
│                         SUPABASE                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │   PostgreSQL Database + REST API + Auth + Storage        │   │
│  │   - 10 tablas relacionadas                               │   │
│  │   - Row Level Security (RLS)                             │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

## Diagrama de Relaciones (10 Tablas)

```
                    ┌──────────────┐
                    │    ROLES     │
                    └──────┬───────┘
                           │ 1:N
                           ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│  CATEGORÍAS  │    │   USUARIOS   │───▶│ DIRECCIONES  │
└──────┬───────┘    └──────┬───────┘    └──────┬───────┘
       │ 1:N               │ 1:N               │
       ▼                   ▼                   │
┌──────────────┐    ┌──────────────┐           │
│  PRODUCTOS   │◀───│   PEDIDOS    │◀──────────┘
└──────┬───────┘    └──────┬───────┘
       │                   │
       │ 1:1               │ 1:N
       ▼                   ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│  INVENTARIO  │    │DETALLE_PEDIDO│    │METODOS_PAGO │
└──────────────┘    └──────────────┘    └──────────────┘

┌──────────────┐
│ PROMOCIONES  │──▶ Puede relacionarse con PRODUCTOS o CATEGORÍAS
└──────────────┘
```

---

## Estructura de Carpetas

```
LaPasadita/
├── Controllers/
│   └── ProductosController.cs      # Controlador CRUD
├── Models/
│   ├── Categoria.cs               # Modelo de categorías
│   ├── Producto.cs                # Modelo de productos
│   ├── Usuario.cs                 # Modelo de usuarios
│   ├── Rol.cs                     # Modelo de roles
│   ├── Pedido.cs                  # Modelo de pedidos
│   ├── DetallesPedido.cs          # Modelo detalle de pedidos
│   ├── Direccion.cs               # Modelo de direcciones
│   ├── Inventario.cs              # Modelo de inventario
│   ├── Promocion.cs               # Modelo de promociones
│   ├── MetodoPago.cs              # Modelo métodos de pago
│   └── DTOs/
│       └── ProductoDTO.cs         # Data Transfer Objects
├── Services/
│   ├── SupabaseService.cs         # Servicio conexión Supabase
│   └── ProductoService.cs         # Servicio de productos
├── Views/
│   ├── Productos/
│   │   └── Index.cshtml           # Vista catálogo con AJAX
│   ├── Shared/
│   │   └── _Layout.cshtml         # Layout principal
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── Database/
│   └── supabase_schema.sql        # Script SQL completo
├── Program.cs                      # Configuración de la app
├── appsettings.json               # Configuración Supabase
└── LaPasadita.csproj              # Archivo del proyecto
```

---

## Cómo Funciona el AJAX

### 1. Carga Inicial
```javascript
document.addEventListener('DOMContentLoaded', function() {
    cargarCategorias();  // GET /api/categorias
    cargarProductos();   // GET /api/productos
});
```

### 2. Búsqueda con Debounce
```javascript
// Espera 300ms después de escribir para hacer la búsqueda
document.getElementById('searchInput').addEventListener('input', function(e) {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(() => {
        buscarProductos(e.target.value);  // GET /api/productos/buscar?q=...
    }, 300);
});
```

### 3. Filtrado por Categoría
```javascript
async function filtrarPorCategoria(categoriaId) {
    const response = await fetch(`/api/productos/categoria/${categoriaId}`);
    const result = await response.json();
    renderizarProductos(result.data);
}
```

---

## Operaciones CRUD

| Operación | Método HTTP | Endpoint                      | Descripción                |
|-----------|-------------|-------------------------------|----------------------------|
| CREATE    | POST        | /api/productos                | Crear nuevo producto       |
| READ      | GET         | /api/productos                | Obtener todos los productos|
| READ      | GET         | /api/productos/{id}           | Obtener producto por ID    |
| READ      | GET         | /api/productos/categoria/{id} | Filtrar por categoría      |
| READ      | GET         | /api/productos/buscar?q=...   | Buscar productos           |
| UPDATE    | PUT         | /api/productos/{id}           | Actualizar producto        |
| DELETE    | DELETE      | /api/productos/{id}           | Eliminar producto (soft)   |

---

## Configuración de Supabase

### 1. Crear proyecto en Supabase
1. Ir a https://supabase.com
2. Crear nuevo proyecto
3. Copiar URL y Anon Key

### 2. Ejecutar script SQL
1. Ir a SQL Editor en Supabase
2. Copiar contenido de `Database/supabase_schema.sql`
3. Ejecutar el script

### 3. Configurar appsettings.json
```json
{
  "Supabase": {
    "Url": "https://TU_PROYECTO.supabase.co",
    "AnonKey": "TU_ANON_KEY_AQUI"
  }
}
```

---

## Pruebas de Funcionamiento

### Verificar API en Navegador:
```
GET https://localhost:5001/api/productos
GET https://localhost:5001/api/categorias
GET https://localhost:5001/api/productos/buscar?q=leche
```

### Verificar con cURL:
```bash
# Obtener todos los productos
curl -X GET https://localhost:5001/api/productos

# Crear producto
curl -X POST https://localhost:5001/api/productos \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Test","precio":10.00,"categoria_id":1}'

# Actualizar producto
curl -X PUT https://localhost:5001/api/productos/1 \
  -H "Content-Type: application/json" \
  -d '{"precio":15.00}'

# Eliminar producto
curl -X DELETE https://localhost:5001/api/productos/1
```

### Verificar en Consola del Navegador:
```javascript
// Probar carga de productos
fetch('/api/productos')
  .then(r => r.json())
  .then(data => console.log('Productos:', data));

// Probar búsqueda
fetch('/api/productos/buscar?q=arroz')
  .then(r => r.json())
  .then(data => console.log('Búsqueda:', data));
```

---

## Comandos para Ejecutar

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar en modo desarrollo
dotnet run

# Compilar para producción
dotnet publish -c Release
```

---

## Tecnologías Utilizadas

| Componente     | Tecnología                    |
|----------------|-------------------------------|
| Backend        | ASP.NET Core MVC 8.0          |
| Frontend       | Razor Views + Tailwind CSS    |
| Base de Datos  | Supabase (PostgreSQL)         |
| Comunicación   | REST API + AJAX (Fetch API)   |
| Serialización  | System.Text.Json              |

---

## Créditos
Proyecto desarrollado para la materia de Desarrollo Web.
Universidad 2024.
