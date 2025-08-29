using Microsoft.EntityFrameworkCore;
using ServicePortal.Infrastructure.Cache;
using ServicePortals.Application.Interfaces.CostCenter;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.CostCenter
{
    public class CostCenterService : ICostCenterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public CostCenterService(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<List<Domain.Entities.CostCenter>> GetAll()
        {
            var result = await _cacheService.GetOrCreateAsync($"get_all_cost_centers", async () =>
            {
                return await _context.CostCenters.ToListAsync();
            }, expireMinutes: 2);

            return result;
        }
    }
}
