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
                        Id = new Guid("11111111-1111-1111-1111-111111111111"),
                        Code = "0",
                        Name = "SuperAdmin",
                        Password = Helper.HashString("123456"),
                        Email = "SuperAdmin@vsvn.com.vn",
                        RoleId = 1,
                        IsActive = true,
                        DateJoinCompany = new DateTime(2025, 4, 1),
                        CreatedAt = new DateTime(2025, 4, 1)
                    }
                );

                await context.SaveChangesAsync();
            }

            if (!context.UserAssignments.Any(e => e.UserCode == "0"))
            {
                context.UserAssignments.Add(new
                    UserAssignment
                    {
                        Id = new Guid("21111111-1111-1111-1111-111111111111"),
                        UserCode = "0",
                        DeparmentId = 1,
                        PositionId = -1,
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
