using System.Globalization;
using ExpenseTracker.Data;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registro del DbContext con la cadena de conexión de appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias: cada vez que se pida IExpenseService se entrega ExpenseService
builder.Services.AddScoped<IExpenseService, ExpenseService>();

var app = builder.Build();

// La base de datos se crea/actualiza con migraciones: Update-Database (Package Manager Console)
// o "dotnet ef database update" (terminal).
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
    app.UseExceptionHandler("/Expenses/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Expenses}/{action=Index}/{id?}");

app.Run();
