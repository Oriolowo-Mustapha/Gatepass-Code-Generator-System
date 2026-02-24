using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(ApplicationDbContext context, IPasswordHasher passwordHasher, IConfiguration configuration)
    {
        if (!await context.Roles.AnyAsync())
        {
            context.Roles.AddRange(
                new Role { RoleName = "Administrator", Permissions = "All" },
                new Role { RoleName = "Security", Permissions = "CanScan,CanViewLogs" },
                new Role { RoleName = "Host", Permissions = "CanApprove,CanViewOwn" },
                new Role { RoleName = "Visitor", Permissions = "CanRequest" }
            );

            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles.FirstAsync(r => r.RoleName == "Administrator");

            var adminConfig = configuration.GetSection("AdminUser");

            context.Users.Add(new User
            {
                FirstName = adminConfig["FirstName"] ?? "Admin",
                LastName = adminConfig["LastName"] ?? "User",
                UserName = adminConfig["UserName"] ?? "admin",
                Email = adminConfig["Email"] ?? "admin@gatepass.edu.ng",
                PhoneNumber = adminConfig["PhoneNumber"] ?? "00000000000",
                PasswordHash = passwordHasher.HashPassword(adminConfig["Password"] ?? "Default@123"),
                RoleId = adminRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        if (!await context.SystemConfigurations.AnyAsync())
        {
            context.SystemConfigurations.AddRange(
                new SystemConfiguration
                {
                    Key = "InstitutionalCodePrefix",
                    Value = "GP",
                    Description = "Prefix for QR Code algorithm"
                },
                new SystemConfiguration
                {
                    Key = "RequireHostApproval",
                    Value = "true",
                    Description = "Whether hosts must approve requests"
                },
                new SystemConfiguration
                {
                    Key = "MaxGatepassDurationHours",
                    Value = "24",
                    Description = "Overstay time limit"
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
