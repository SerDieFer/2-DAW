using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Vestigio.Data;
using Vestigio.Utilities;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ??
    //throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

// GET THE PATH TO THE CURRENT USER'S DESKTOP DIRECTORY
string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

// CHECK IF USER PROFILE IS NULL OR EMPTY
if (string.IsNullOrEmpty(userProfile))
{
    // IF THE DESKTOP CANNOT BE OBTAINED, USE THE CURRENT PROJECT DIRECTORY AS BACKUP
    Console.WriteLine("No se pudo obtener la ruta del escritorio. Usando el directorio actual del proyecto.");
    userProfile = Directory.GetCurrentDirectory();  // WE USE THE CURRENT PROJECT DIRECTORY
}

// DEFINE THE PATH TO THE DATABASE DEPENDING ON THE FOLDER STRUCTURE
string dbFolderPath;

// CHECK IF THE �2-DAW� FOLDER EXISTS ON THE DESKTOP
if (Directory.Exists(Path.Combine(userProfile, "2-DAW")))
{
    dbFolderPath = Path.Combine(userProfile, "2-DAW", "SERVER", "DBMIGRATIONS");
}

// CHECK IF THE FOLDER �DAW� EXISTS ON THE DESKTOP AND CONTAINS �2-DAW�.
else if (Directory.Exists(Path.Combine(userProfile, "DAW")))
{
    // CHOOSE �2-DAW�, SET IF ANOTHER FOLDER IS NEEDED
    dbFolderPath = Path.Combine(userProfile, "DAW", "2-DAW", "SERVER", "DBMIGRATIONS");
}
else
{
    throw new DirectoryNotFoundException("No se encontro la carpeta '2-DAW' ni 'DAW' en el escritorio.");
}

// MAKE SURE THAT THE FOLDER EXISTS, IF NOT, CREATE IT
if (!Directory.Exists(dbFolderPath))
{
    Directory.CreateDirectory(dbFolderPath);
}

var dbFileName = "aspnet-Vestigio-592bae53-5786-46d1-acfd-d411bbb0174c.mdf"; // NAME OF THE MDF FILE

// GENERATES THE FULL PATH TO THE .MDF FILE
var dbPath = Path.Combine(dbFolderPath, dbFileName);

// BUILD THE CONNECTION STRING USING THE ABSOLUTE ROUTE
var connectionString = $"Server=(localdb)\\MSSQL15;AttachDbFileName={dbPath};Database=aspnet-Vestigio;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// REGISTER THE DATABASE CONTEXT 
builder.Services.AddDbContext<VestigioDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// UNABLE USER CONFIRMATION, IT USES IDENTITY CONFIG
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();


// ASP.NET CORE IDENTITY SERVICES CONFIGURATION
builder.Services.Configure<IdentityOptions>(options =>
{
    // PASSWORD SETTINGS.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    //options.Password.RequireNonAlphanumeric = true; 
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
});


var app = builder.Build();

// CONFIGURE THE HTTP REQUEST PIPELINE.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // THE DEFAULT HSTS VALUE IS 30 DAYS. YOU MAY WANT TO CHANGE THIS FOR PRODUCTION SCENARIOS, SEE HTTPS://AKA.MS/ASPNETCORE-HSTS.
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

// ROLE CREATION AND ADMIN SETTING
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.InitializeAsync(services).Wait();
}

app.Run();
