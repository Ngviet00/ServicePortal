using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.CostCenter;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.CostCenter
{
    public class CostCenterService : ICostCenterService
    {
        private readonly ApplicationDbContext _context;

        public CostCenterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Domain.Entities.CostCenter>> GetAll()
        {
            return await _context.CostCenters.ToListAsync();
        }
    }
}
