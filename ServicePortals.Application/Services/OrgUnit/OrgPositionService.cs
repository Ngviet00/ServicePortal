using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.OrgUnit
{
    public class OrgPositionService : IOrgPositionService
    {
        private readonly ApplicationDbContext _context;

        public OrgPositionService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrgPosition>> GetOrgPositionsByDepartmentId(int departmentId)
        {
            var results = await _context.OrgPositions.Where(e => e.OrgUnit != null && (e.OrgUnit.Id == departmentId || e.OrgUnit.ParentOrgUnitId == departmentId)).ToListAsync();

            return results;
        }
    }
}
