using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using TipCalculator.Data;
using TipCalculator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registro del DbContext con la cadena de conexión de appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias: cada vez que se pida ITipService se entrega TipService
builder.Services.AddScoped<ITipService, TipService>();

var app = builder.Build();

// La base de datos se crea/actualiza con migraciones: Update-Database (Package Manager Console).
// Opcional: descomenta este bloque para aplicar las migraciones pendientes al iniciar la app.
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

// Fijamos la cultura para que los decimales se lean siempre con punto (50.5),
// que es lo que envía el navegador en los campos type="number".
var culture = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new List<CultureInfo> { culture },
    SupportedUICultures = new List<CultureInfo> { culture }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Tip/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tip}/{action=Index}/{id?}");

app.Run();
