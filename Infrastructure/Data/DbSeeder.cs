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

                var positionDeparment = new PositionDeparment
                {
                    DeparmentId = 1,
                    PositionId = -1,
                    PositionDeparmentLevel = 0,
                    CustomTitle = "SuperAdmin"
                };

                await context.SaveChangesAsync();

                context.UserAssignments.Add(new 
                    UserAssignment
                    {
                        UserCode = "0",
                        PositionDeparmentId = positionDeparment.Id,
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
