using LaPasadita.Services;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN DE SERVICIOS
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ISupabaseService, SupabaseService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

// PIPELINE DE MIDDLEWARE
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

// RUTAS
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "home",
    pattern: "Home/{action=Index}/{id?}",
    defaults: new { controller = "Home" });

app.Run();
