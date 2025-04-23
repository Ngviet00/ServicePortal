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
                        Name = "Superadmin",
                        Password = Helper.HashString("123456"),
                        Email = "superadmin@vsvn.com.vn",
                        RoleId = 1,
                        IsActive = true,
                        DateJoinCompany = new DateTime(2025, 4, 1),
                        DateOfBirth = new DateTime(2020, 5, 2),
                        Phone = "0345248120",
                        Sex = 1,
                        ParentDepartmentId = 1,
                        PositionId = 3,
                        TeamId = null,
                        CreatedAt = new DateTime(2025, 4, 1)
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
