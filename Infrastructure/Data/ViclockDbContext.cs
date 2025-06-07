using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Infrastructure.Data
{
    public class ViclockDbContext : DbContext
    {
        public ViclockDbContext(DbContextOptions<ViclockDbContext> options) : base(options) { }
    }
}
