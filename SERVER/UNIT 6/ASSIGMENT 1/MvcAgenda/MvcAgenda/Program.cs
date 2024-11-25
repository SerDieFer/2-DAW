using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcAgenda.Data;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Obtener la ruta al directorio "Desktop" del usuario actual
string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

// Verificar si userProfile es null o vacío
if (string.IsNullOrEmpty(userProfile))
{
    // Si no se puede obtener el escritorio, usar el directorio actual del proyecto como respaldo
    Console.WriteLine("No se pudo obtener la ruta del escritorio. Usando el directorio actual del proyecto.");
    userProfile = Directory.GetCurrentDirectory();  // Usamos el directorio actual del proyecto
}

// Definir la ruta a la base de datos dependiendo de la estructura de carpetas
string dbFolderPath;

// Verificar si la carpeta "2-DAW" existe en el escritorio
if (Directory.Exists(Path.Combine(userProfile, "2-DAW")))
{
    dbFolderPath = Path.Combine(userProfile, "2-DAW", "SERVER", "DBMIGRATIONS");
}

// Verificar si la carpeta "DAW" existe en el escritorio y contiene "2-DAW"
else if (Directory.Exists(Path.Combine(userProfile, "DAW")))
{
    // Elegir "2-DAW", ajustar si se necesita otra carpeta
    dbFolderPath = Path.Combine(userProfile, "DAW", "2-DAW", "SERVER", "DBMIGRATIONS");
}
else
{
    throw new DirectoryNotFoundException("No se encontró la carpeta '2-DAW' ni 'DAW' en el escritorio.");
}

// Asegurarse de que la carpeta exista, si no, crearla
if (!Directory.Exists(dbFolderPath))
{
    Directory.CreateDirectory(dbFolderPath);
}

var dbFileName = "aspnet-MvcAgenda-e4d88d97-9f8f-4965-987e-c65ea2ce259f.mdf"; // Nombre del archivo MDF

// Genera la ruta completa para el archivo .mdf
var dbPath = Path.Combine(dbFolderPath, dbFileName);

// Construir la cadena de conexión utilizando la ruta absoluta
var connectionString = $"Server=(localdb)\\MSSQL15;AttachDbFileName={dbPath};Database=aspnet-MvcAgenda;Trusted_Connection=True;MultipleActiveResultSets=true";

// Configura el DbContext con la nueva cadena de conexión
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Configuración de DbContext adicional
builder.Services.AddDbContext<MvcAgendaContexto>(options => options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();    
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
