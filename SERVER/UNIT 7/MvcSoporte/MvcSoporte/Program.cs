using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcSoporte;
using MvcSoporte.Data;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

// OBTENER LA RUTA AL DIRECTORIO "DESKTOP" DEL USUARIO ACTUAL
string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

// VERIFICAR SI USERPROFILE ES NULL O VAC�O
if (string.IsNullOrEmpty(userProfile))
{
    // SI NO SE PUEDE OBTENER EL ESCRITORIO, USAR EL DIRECTORIO ACTUAL DEL PROYECTO COMO RESPALDO
    Console.WriteLine("No se pudo obtener la ruta del escritorio. Usando el directorio actual del proyecto.");
    userProfile = Directory.GetCurrentDirectory();  // USAMOS EL DIRECTORIO ACTUAL DEL PROYECTO
}

// DEFINIR LA RUTA A LA BASE DE DATOS DEPENDIENDO DE LA ESTRUCTURA DE CARPETAS
string dbFolderPath;

// VERIFICAR SI LA CARPETA "2-DAW" EXISTE EN EL ESCRITORIO
if (Directory.Exists(Path.Combine(userProfile, "2-DAW")))
{
    dbFolderPath = Path.Combine(userProfile, "2-DAW", "SERVER", "DBMIGRATIONS");
}

// VERIFICAR SI LA CARPETA "DAW" EXISTE EN EL ESCRITORIO Y CONTIENE "2-DAW"
else if (Directory.Exists(Path.Combine(userProfile, "DAW")))
{
    // ELEGIR "2-DAW", AJUSTAR SI SE NECESITA OTRA CARPETA
    dbFolderPath = Path.Combine(userProfile, "DAW", "2-DAW", "SERVER", "DBMIGRATIONS");
}
else
{
    throw new DirectoryNotFoundException("No se encontr� la carpeta '2-DAW' ni 'DAW' en el escritorio.");
}

// ASEGURARSE DE QUE LA CARPETA EXISTA, SI NO, CREARLA
if (!Directory.Exists(dbFolderPath))
{
    Directory.CreateDirectory(dbFolderPath);
}

var dbFileName = "aspnet-MvcSoporte-edda19e4-4c2b-4592-aa7a-c1d1bf7420f8.mdf"; // NOMBRE DEL ARCHIVO MDF

// GENERA LA RUTA COMPLETA PARA EL ARCHIVO .MDF
var dbPath = Path.Combine(dbFolderPath, dbFileName);

// CONSTRUIR LA CADENA DE CONEXI�N UTILIZANDO LA RUTA ABSOLUTA
var connectionString = $"Server=(localdb)\\MSSQL15;AttachDbFileName={dbPath};Database=aspnet-MvcSoporte;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// REGISTRAR EL CONTEXTO DE LA BASE DE DATOS 
builder.Services.AddDbContext<MvcSoporteContexto>(options => options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// CONFIGURACIÓN DE LOS SERVICIOS DE ASP.NET CORE IDENTITY 
builder.Services.Configure<IdentityOptions>(options =>
{
    // PASSWORD SETTINGS. CONFIGURACIÓN DE LAS CARACTERÍSTICAS DE LAS CONTRASEÑAS 
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    //options.Password.RequireNonAlphanumeric = true; 
    options.Password.RequireNonAlphanumeric = false;
    //options.Password.RequireUppercase = true; 
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

// CREAR LOS ROLES Y EL ADMINISTRADOR PREDETERMINADOS 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.InitializeAsync(services).Wait();
}

app.Run();
