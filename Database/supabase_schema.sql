-- ============================================
-- SCRIPT SQL PARA SUPABASE - LA PASADITA
-- Ejecutar en el SQL Editor de Supabase
-- ============================================

-- 1. TABLA: Roles
CREATE TABLE IF NOT EXISTS roles (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL UNIQUE,
    descripcion TEXT,
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 2. TABLA: Usuarios
CREATE TABLE IF NOT EXISTS usuarios (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido_paterno VARCHAR(100) NOT NULL,
    apellido_materno VARCHAR(100),
    email VARCHAR(255) NOT NULL UNIQUE,
    telefono VARCHAR(20),
    password_hash VARCHAR(255) NOT NULL,
    rol_id INTEGER REFERENCES roles(id),
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    ultimo_acceso TIMESTAMP WITH TIME ZONE
);

-- 3. TABLA: Categorías
CREATE TABLE IF NOT EXISTS categorias (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL UNIQUE,
    descripcion TEXT,
    imagen_url TEXT,
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 4. TABLA: Productos
CREATE TABLE IF NOT EXISTS productos (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    precio DECIMAL(10,2) NOT NULL,
    precio_oferta DECIMAL(10,2),
    imagen_url TEXT,
    codigo_barras VARCHAR(50),
    marca VARCHAR(100),
    unidad_medida VARCHAR(50),
    categoria_id INTEGER REFERENCES categorias(id),
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    fecha_actualizacion TIMESTAMP WITH TIME ZONE
);

-- 5. TABLA: Inventario
CREATE TABLE IF NOT EXISTS inventario (
    id SERIAL PRIMARY KEY,
    producto_id INTEGER UNIQUE REFERENCES productos(id) ON DELETE CASCADE,
    cantidad_disponible INTEGER NOT NULL DEFAULT 0,
    cantidad_minima INTEGER DEFAULT 5,
    cantidad_maxima INTEGER DEFAULT 100,
    ubicacion VARCHAR(100),
    ultima_entrada TIMESTAMP WITH TIME ZONE,
    ultima_salida TIMESTAMP WITH TIME ZONE,
    fecha_actualizacion TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 6. TABLA: Direcciones
CREATE TABLE IF NOT EXISTS direcciones (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER REFERENCES usuarios(id) ON DELETE CASCADE,
    calle VARCHAR(200) NOT NULL,
    numero_exterior VARCHAR(20) NOT NULL,
    numero_interior VARCHAR(20),
    colonia VARCHAR(100) NOT NULL,
    codigo_postal VARCHAR(10) NOT NULL,
    ciudad VARCHAR(100) NOT NULL,
    estado VARCHAR(100) NOT NULL,
    referencias TEXT,
    es_principal BOOLEAN DEFAULT false,
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 7. TABLA: Métodos de Pago
CREATE TABLE IF NOT EXISTS metodos_pago (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL UNIQUE,
    descripcion TEXT,
    icono_url TEXT,
    requiere_datos_adicionales BOOLEAN DEFAULT false,
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 8. TABLA: Pedidos
CREATE TABLE IF NOT EXISTS pedidos (
    id SERIAL PRIMARY KEY,
    numero_orden VARCHAR(20) NOT NULL UNIQUE,
    usuario_id INTEGER REFERENCES usuarios(id),
    direccion_id INTEGER REFERENCES direcciones(id),
    metodo_pago_id INTEGER REFERENCES metodos_pago(id),
    subtotal DECIMAL(10,2) NOT NULL,
    descuento DECIMAL(10,2) DEFAULT 0,
    costo_envio DECIMAL(10,2) DEFAULT 0,
    total DECIMAL(10,2) NOT NULL,
    estado VARCHAR(50) DEFAULT 'Pendiente',
    notas TEXT,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    fecha_entrega TIMESTAMP WITH TIME ZONE,
    fecha_actualizacion TIMESTAMP WITH TIME ZONE
);

-- 9. TABLA: Detalle de Pedidos
CREATE TABLE IF NOT EXISTS detalle_pedidos (
    id SERIAL PRIMARY KEY,
    pedido_id INTEGER REFERENCES pedidos(id) ON DELETE CASCADE,
    producto_id INTEGER REFERENCES productos(id),
    cantidad INTEGER NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    descuento DECIMAL(10,2) DEFAULT 0,
    subtotal DECIMAL(10,2) NOT NULL,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 10. TABLA: Promociones
CREATE TABLE IF NOT EXISTS promociones (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    tipo_descuento VARCHAR(20) DEFAULT 'Porcentaje',
    valor_descuento DECIMAL(10,2) NOT NULL,
    producto_id INTEGER REFERENCES productos(id),
    categoria_id INTEGER REFERENCES categorias(id),
    fecha_inicio TIMESTAMP WITH TIME ZONE NOT NULL,
    fecha_fin TIMESTAMP WITH TIME ZONE NOT NULL,
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ============================================
-- ÍNDICES PARA MEJORAR RENDIMIENTO
-- ============================================
CREATE INDEX IF NOT EXISTS idx_productos_categoria ON productos(categoria_id);
CREATE INDEX IF NOT EXISTS idx_productos_activo ON productos(activo);
CREATE INDEX IF NOT EXISTS idx_pedidos_usuario ON pedidos(usuario_id);
CREATE INDEX IF NOT EXISTS idx_pedidos_estado ON pedidos(estado);
CREATE INDEX IF NOT EXISTS idx_inventario_producto ON inventario(producto_id);

-- ============================================
-- DATOS DE PRUEBA
-- ============================================

-- Insertar roles
INSERT INTO roles (nombre, descripcion) VALUES
    ('Admin', 'Administrador del sistema'),
    ('Cliente', 'Cliente de la tienda'),
    ('Empleado', 'Empleado de la tienda')
ON CONFLICT (nombre) DO NOTHING;

-- Insertar categorías
INSERT INTO categorias (nombre, descripcion, imagen_url) VALUES
    ('Abarrotes', 'Productos básicos de despensa', 'https://images.unsplash.com/photo-1604719312566-8912e9227c6a?w=400'),
    ('Lácteos', 'Leche, quesos y derivados', 'https://images.unsplash.com/photo-1550583724-b2692b85b150?w=400'),
    ('Bebidas', 'Refrescos, jugos y agua', 'https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=400'),
    ('Carnes Frías', 'Jamón, salchichas y embutidos', 'https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=400'),
    ('Panadería', 'Pan, galletas y repostería', 'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400'),
    ('Limpieza', 'Productos de limpieza del hogar', 'https://images.unsplash.com/photo-1563453392212-326f5e854473?w=400'),
    ('Higiene Personal', 'Jabones, shampoo y cuidado personal', 'https://images.unsplash.com/photo-1556228720-195a672e8a03?w=400'),
    ('Frutas y Verduras', 'Productos frescos del campo', 'https://images.unsplash.com/photo-1610832958506-aa56368176cf?w=400')
ON CONFLICT (nombre) DO NOTHING;

-- Insertar productos de ejemplo
INSERT INTO productos (nombre, descripcion, precio, precio_oferta, imagen_url, marca, unidad_medida, categoria_id, activo) VALUES
    ('Arroz La Merced 1kg', 'Arroz blanco de grano largo, ideal para todo tipo de platillos', 28.50, NULL, 'https://images.unsplash.com/photo-1586201375761-83865001e31c?w=400', 'La Merced', 'Kilogramo', 1, true),
    ('Frijol Negro 1kg', 'Frijol negro de primera calidad, cosecha reciente', 35.00, 32.00, 'https://images.unsplash.com/photo-1551462147-ff29053bfc14?w=400', 'Verde Valle', 'Kilogramo', 1, true),
    ('Aceite Vegetal 1L', 'Aceite vegetal puro para cocinar', 42.00, NULL, 'https://images.unsplash.com/photo-1474979266404-7eaacbcd87c5?w=400', '1-2-3', 'Litro', 1, true),
    ('Azúcar Estándar 1kg', 'Azúcar refinada para uso diario', 32.00, NULL, 'https://images.unsplash.com/photo-1558642452-9d2a7deb7f62?w=400', 'Zulka', 'Kilogramo', 1, true),
    ('Leche Entera 1L', 'Leche fresca pasteurizada', 24.50, NULL, 'https://images.unsplash.com/photo-1563636619-e9143da7973b?w=400', 'Lala', 'Litro', 2, true),
    ('Queso Panela 400g', 'Queso fresco bajo en grasa', 65.00, 58.00, 'https://images.unsplash.com/photo-1486297678162-eb2a19b0a32d?w=400', 'Los Volcanes', 'Pieza', 2, true),
    ('Yogurt Natural 1kg', 'Yogurt natural sin azúcar añadida', 45.00, NULL, 'https://images.unsplash.com/photo-1488477181946-6428a0291777?w=400', 'Yoplait', 'Kilogramo', 2, true),
    ('Coca-Cola 2L', 'Refresco de cola clásico', 32.00, NULL, 'https://images.unsplash.com/photo-1629203851122-3726ecdf080e?w=400', 'Coca-Cola', 'Botella', 3, true),
    ('Agua Natural 1.5L', 'Agua purificada embotellada', 12.00, NULL, 'https://images.unsplash.com/photo-1548839140-29a749e1cf4d?w=400', 'Ciel', 'Botella', 3, true),
    ('Jugo de Naranja 1L', 'Jugo 100% natural de naranja', 28.00, 25.00, 'https://images.unsplash.com/photo-1621506289937-a8e4df240d0b?w=400', 'Del Valle', 'Litro', 3, true),
    ('Jamón de Pavo 250g', 'Jamón de pavo bajo en grasa', 48.00, NULL, 'https://images.unsplash.com/photo-1544025162-d76694265947?w=400', 'FUD', 'Paquete', 4, true),
    ('Salchichas de Pavo', 'Paquete de 10 salchichas de pavo', 55.00, 49.00, 'https://images.unsplash.com/photo-1509482560494-4126f8225994?w=400', 'San Rafael', 'Paquete', 4, true),
    ('Pan Blanco Bimbo', 'Pan de caja blanco grande', 52.00, NULL, 'https://images.unsplash.com/photo-1549931319-a545dcf3bc73?w=400', 'Bimbo', 'Paquete', 5, true),
    ('Galletas Marías 500g', 'Galletas clásicas para toda la familia', 25.00, NULL, 'https://images.unsplash.com/photo-1558961363-fa8fdf82db35?w=400', 'Gamesa', 'Paquete', 5, true),
    ('Detergente Líquido 1L', 'Detergente para ropa concentrado', 65.00, 59.00, 'https://images.unsplash.com/photo-1585659722983-3a675dabf23d?w=400', 'Ariel', 'Botella', 6, true),
    ('Jabón en Polvo 1kg', 'Jabón para lavadora de ropa', 38.00, NULL, 'https://images.unsplash.com/photo-1583947215259-38e31be8751f?w=400', 'Roma', 'Bolsa', 6, true),
    ('Shampoo 400ml', 'Shampoo para cabello normal', 72.00, NULL, 'https://images.unsplash.com/photo-1535585209827-a15fcdbc4c2d?w=400', 'Head & Shoulders', 'Botella', 7, true),
    ('Jabón de Tocador 3 pzas', 'Jabón antibacterial para manos y cuerpo', 28.00, 24.00, 'https://images.unsplash.com/photo-1584305574647-0cc949a2bb9f?w=400', 'Escudo', 'Paquete', 7, true),
    ('Plátano Tabasco 1kg', 'Plátanos frescos y maduros', 18.00, NULL, 'https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=400', 'Granel', 'Kilogramo', 8, true),
    ('Jitomate Saladet 1kg', 'Jitomates rojos para ensalada', 25.00, NULL, 'https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=400', 'Granel', 'Kilogramo', 8, true)
ON CONFLICT DO NOTHING;

-- Insertar métodos de pago
INSERT INTO metodos_pago (nombre, descripcion) VALUES
    ('Efectivo', 'Pago en efectivo al momento de la entrega'),
    ('Tarjeta de Débito', 'Pago con tarjeta de débito'),
    ('Tarjeta de Crédito', 'Pago con tarjeta de crédito'),
    ('Transferencia', 'Transferencia bancaria SPEI')
ON CONFLICT (nombre) DO NOTHING;

-- ============================================
-- HABILITAR ROW LEVEL SECURITY (RLS)
-- ============================================

-- Habilitar RLS en todas las tablas
ALTER TABLE productos ENABLE ROW LEVEL SECURITY;
ALTER TABLE categorias ENABLE ROW LEVEL SECURITY;

-- Política para permitir lectura pública de productos y categorías
CREATE POLICY "Productos visibles para todos" ON productos
    FOR SELECT USING (activo = true);

CREATE POLICY "Categorías visibles para todos" ON categorias
    FOR SELECT USING (activo = true);

-- Política para permitir todas las operaciones (desarrollo)
CREATE POLICY "Permitir todo en productos" ON productos
    FOR ALL USING (true) WITH CHECK (true);

CREATE POLICY "Permitir todo en categorias" ON categorias
    FOR ALL USING (true) WITH CHECK (true);

-- ============================================
-- VERIFICACIÓN DE DATOS
-- ============================================
SELECT 'Categorías creadas: ' || COUNT(*) FROM categorias;
SELECT 'Productos creados: ' || COUNT(*) FROM productos;
SELECT 'Roles creados: ' || COUNT(*) FROM roles;
