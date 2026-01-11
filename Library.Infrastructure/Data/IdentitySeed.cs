using Microsoft.AspNetCore.Identity;

namespace Library.Infrastructure.Identity;

public static class IdentitySeed
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // ROLE
        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // ADMIN
        var adminEmail = "admin@gmail.com";
        var adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                throw new Exception(
                    "Nie udało się stworzyć admina: " +
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }
        }
    }
}
