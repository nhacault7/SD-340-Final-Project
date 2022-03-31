using Finalproject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Finalproject.Models
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            List<string> roles = new List<string>()
            {
                "Admin",
                "Project Manager",
                "Developer",
            };

            if (!context.Roles.Any())
            {
                foreach (string role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                ApplicationUser seededUser = new ApplicationUser
                {
                    Email = "Admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    UserName = "Admin@gmail.com",
                    NormalizedUserName = "ADMIN@GMAIL.COM",
                    EmailConfirmed = true,
                };

                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(seededUser, "P@ssword1");
                seededUser.PasswordHash = hashed;

                await userManager.CreateAsync(seededUser);
                await userManager.AddToRoleAsync(seededUser, "Admin");

            }

            await context.SaveChangesAsync();
        }
    }


}
