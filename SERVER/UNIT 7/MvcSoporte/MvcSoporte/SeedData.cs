using Microsoft.AspNetCore.Identity;

namespace MvcSoporte
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            // COMPROBAR Y CREAR LOS ROLES PREDETERMINADOS 
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await CrearRolesAsync(roleManager);

            // COMPROBAR Y CREAR EL ADMINISTRADOR PREDETERMINADO 
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            await CrearAdminAsync(userManager);
        }

        private static async Task CrearRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // SI NO EXISTE, SE CREA EL ROL PREDETERMINADO "ADMINISTRADOR" 
            string nombreRol = "Administrador";
            var yaExiste = await roleManager.RoleExistsAsync(nombreRol);
            if (!yaExiste)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));

            // SI NO EXISTE, SE CREA EL ROL PREDETERMINADO "USUARIO" 
            nombreRol = "Usuario";
            yaExiste = await roleManager.RoleExistsAsync(nombreRol);
            if (!yaExiste)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));
        }

        private static async Task CrearAdminAsync(UserManager<IdentityUser> userManager)
        {
            // COMPROBAR SI EXISTE EL ADMINISTRADOR PREDETERMINDADO 
            var testAdmin = userManager.Users
                    .Where(x => x.UserName == "admin@empresa.com")
                    .SingleOrDefault();

            if (testAdmin != null) return;

            testAdmin = new IdentityUser
            {
                UserName = "admin@empresa.com",
                Email = "admin@empresa.com"
            };
            string admPasswd = "Admin-123";

            // SI NO EXISTE, SE CREA EL ADMINISTRADOR PREDETERMINADO "ADMIN@EMPRESA.COM" 
            IdentityResult userResult;
            userResult = await userManager.CreateAsync(testAdmin, admPasswd);

            // SE AGREGA EL ROL "ADMINISTRADOR" AL ADMINISTRADOR PREDETERMINADO  
            if (userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testAdmin, "Administrador");
            }
        }
    }
}
