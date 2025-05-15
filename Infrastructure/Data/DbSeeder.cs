using ServicePortal.Common.Helpers;
using ServicePortal.Domain.Entities;

namespace ServicePortal.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any(e => e.UserCode == "0"))
            {
                context.Users.Add(
                    new User
                    {
                        UserCode = "0",
                        Password = Helper.HashString("123456"),
                        PositionId = 0,
                        IsChangePassword = 1,
                        IsActive = 1,
                    }
                );

                context.UserRoles.Add(new UserRole
                {
                    UserCode = "0",
                    RoleId = 1,
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
