using Microsoft.AspNetCore.Identity;

namespace Vestigio.Utilities
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

        private static async Task CrearAdminAsync(UserManager<IdentityUser> userManager)
        {
            // COMPROBAR SI EXISTE EL ADMINISTRADOR PREDETERMINDADO
            var testAdmin = userManager.Users
                    .Where(x => x.UserName == "admin@vestigio.com")
                    .SingleOrDefault();

            if (testAdmin != null) return;

            testAdmin = new IdentityUser
            {
                UserName = "admin@vestigio.com",
                Email = "admin@vestigio.com"
            };

            string admPasswd = "Vestigio-123";

            // SI NO EXISTE, SE CREA EL ADMINISTRADOR PREDETERMINADO "ADMIN@VESTIGIO.COM"
            IdentityResult userResult;
            userResult = await userManager.CreateAsync(testAdmin, admPasswd);

            // SE AGREGA EL ROL "ADMINISTRADOR" AL ADMINISTRADOR PREDETERMINADO
            if (userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testAdmin, "Admin");
            }
        }

        private static async Task CrearRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // SI NO EXISTE, SE CREA EL ROL PREDETERMINADO "ADMIN"
            string rolName = "Admin";
            var alreadyExist = await roleManager.RoleExistsAsync(rolName);
            if (!alreadyExist)
                await roleManager.CreateAsync(new IdentityRole(rolName));

            // SI NO EXISTE, SE CREA EL ROL PREDETERMINADO "USER"
            rolName = "User";
            alreadyExist = await roleManager.RoleExistsAsync(rolName);
            if (!alreadyExist)
                await roleManager.CreateAsync(new IdentityRole(rolName));
        }
    }
}