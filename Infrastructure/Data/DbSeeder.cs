namespace ServicePortal.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static Task SeedAsync(ApplicationDbContext context)
        {
            return Task.CompletedTask;
        }
    }
}
