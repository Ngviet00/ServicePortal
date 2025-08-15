using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.OrgUnit
{
    public class PositionService : IPositionService
    {
        private readonly ApplicationDbContext _context;

        public PositionService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Position>> GetPositionsByDepartmentId(int departmentId)
        {
            var results = await _context.Positions.Where(e => e.OrgUnit != null && (e.OrgUnit.Id == departmentId || e.OrgUnit.ParentOrgUnitId == departmentId)).ToListAsync();

            return results;
        }
    }
}
