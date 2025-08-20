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

        public async Task<OrgPosition?> GetManagerOrgPostionIdByOrgPositionId(int orgPostionId)
        {
            var manager = await _context.OrgPositions
                .FromSqlRaw(@"
                    WITH ParentPositions AS (
                        SELECT Id, PositionCode, Name, ParentOrgPositionId
                        FROM org_positions
                        WHERE Id = {0}
                        UNION ALL
                        SELECT o.Id, o.PositionCode, o.Name, o.ParentOrgPositionId
                        FROM org_positions o
                        INNER JOIN ParentPositions p ON o.Id = p.ParentOrgPositionId
                    )
                    SELECT TOP 1 * FROM ParentPositions WHERE ParentOrgPositionId IS NULL
                ", orgPostionId)
                .FirstOrDefaultAsync();

            return manager;
        }

        public async Task<List<OrgPosition>> GetOrgPositionsByDepartmentId(int departmentId)
        {
            var results = await _context.OrgPositions.Where(e => e.OrgUnit != null && (e.OrgUnit.Id == departmentId || e.OrgUnit.ParentOrgUnitId == departmentId)).ToListAsync();

            return results;
        }
    }
}
