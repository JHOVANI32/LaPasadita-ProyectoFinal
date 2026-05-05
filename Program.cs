using LaPasadita.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURACIÓN DE SERVICIOS
// ============================================

// Agregar MVC con vistas Razor
builder.Services.AddControllersWithViews();

// Registrar servicios de Supabase (Inyección de Dependencias)
builder.Services.AddSingleton<ISupabaseService, SupabaseService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// Configurar CORS para permitir llamadas AJAX
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar JSON para respuestas de API
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

var app = builder.Build();

// ============================================
// PIPELINE DE MIDDLEWARE
// ============================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

// ============================================
// RUTAS
// ============================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}");

app.Run();
