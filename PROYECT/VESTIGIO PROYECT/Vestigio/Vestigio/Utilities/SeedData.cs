using Microsoft.AspNetCore.Identity;
using Vestigio.Models;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        // Create default roles
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await CreateRolesAsync(roleManager);

        // Create default admin user
        var userManager = services.GetRequiredService<UserManager<User>>();
        await CreateAdminAsync(userManager);
    }

    private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // Create "Admin" role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create "User" role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }

    private static async Task CreateAdminAsync(UserManager<User> userManager)
    {
        string adminEmail = "admin@vestigio.com";
        string adminNickname = "AdminMaster";

        // Check if the admin user already exists
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true, // Skip email confirmation for the admin
                Nickname = adminNickname
            };

            // Create the admin user with a strong password
            var result = await userManager.CreateAsync(adminUser, "Vestigio-123");
            if (result.Succeeded)
            {
                // Assign the "Admin" role to the user
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}