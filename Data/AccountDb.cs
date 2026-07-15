using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookstoreCatalog.Mvc.Models;
using System.Net;
using System.Data;
using System.Dynamic;

namespace BookstoreCatalog.Mvc.Data;

public static class AccountDb
{
    public static async Task SeedIdentityAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "Staff", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await CreateUser(userManager, "admin@bookstore.test", "Admin@123", "Admin");
        await CreateUser(userManager, "staff@bookstore.test", "Staff@123", "Staff");
        await CreateUser(userManager, "user@bookstore.test", "User@123", "User");
    }

    private static async Task CreateUser(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser 
            { 
                UserName = email, 
                Email = email, 
                EmailConfirmed = true, 
                FullName = role + " Demo" 
            };
            
            var result = await userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Không thể tạo user {email}. Lý do: {errors}");
            }
        }
    }
}