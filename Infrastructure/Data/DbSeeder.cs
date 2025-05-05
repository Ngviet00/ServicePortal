using ServicePortal.Common.Helpers;
using ServicePortal.Domain.Entities;

namespace ServicePortal.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any(e => e.Code == "0"))
            {
                context.Users.Add(
                    new User
                    {
                        Code = "0",
                        Name = "superadmin",
                        Password = Helper.HashString("123456"),
                        Email = "superadmin@vsvn.com.vn",
                        IsActive = true,
                        DateJoinCompany = new DateTime(2025, 4, 1),
                        DateOfBirth = new DateTime(2020, 5, 2),
                        Phone = "123456789",
                        Sex = 1,
                        DepartmentId = 1,
                        Level = "0",
                        Position = "superadmin",
                        CreatedAt = new DateTime(2025, 4, 1)
                    }
                );

                context.UserRoles.Add(new UserRole
                {
                    UserCode = "0",
                    RoleId = 1,
                    DepartmentId = 1
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
