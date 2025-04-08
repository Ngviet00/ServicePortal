using ServicePortal.Common.Helper;
using ServicePortal.Domains.Models;

namespace ServicePortal.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.User.Any(e => e.Name == "admin"))
            {
                context.User.Add(new User
                {
                    Id = Guid.NewGuid(),
                    EmployeeCode = "0",
                    Name = "admin",
                    Password = Helper.HashString("123456"),
                    Email = "admin@vsvn.com",
                    Deparment = "IT",
                    Title = "Admin",
                    Role = 1,
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
