using Microsoft.EntityFrameworkCore;
using OnlineChronometer.Data;
using OnlineChronometer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// El JavaScript del cronómetro envía el token antiforgery en este encabezado
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

// Registro del DbContext con la cadena de conexión de appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias: cada vez que se pida IChronometerService se entrega ChronometerService
builder.Services.AddScoped<IChronometerService, ChronometerService>();

var app = builder.Build();

// La base de datos se crea/actualiza con migraciones: Update-Database (Package Manager Console)
// o "dotnet ef database update" (terminal).
// Opcional: descomenta este bloque para aplicar las migraciones pendientes al iniciar la app.
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Chronometers/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chronometers}/{action=Index}/{id?}");

app.Run();
