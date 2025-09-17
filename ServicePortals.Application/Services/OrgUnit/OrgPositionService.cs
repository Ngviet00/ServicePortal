using Dapper;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
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
            var result = await _context.Database.GetDbConnection().QueryFirstOrDefaultAsync<OrgPosition>($@"
                WITH ParentPositions AS (
                    SELECT Id, PositionCode, Name, ParentOrgPositionId, UnitId
                    FROM org_positions
                    WHERE Id = @OrgPositionId
                    UNION ALL
                    SELECT o.Id, o.PositionCode, o.Name, o.ParentOrgPositionId, o.UnitId
                    FROM org_positions o
                    INNER JOIN ParentPositions p ON o.Id = p.ParentOrgPositionId
                )
                SELECT TOP 1 * FROM ParentPositions WHERE UnitId = 6
                ", new { OrgPositionId = orgPostionId });

            return result;
        }

        public async Task<List<OrgPosition>> GetOrgPositionsByDepartmentId(int? departmentId)
        {
            var query = _context.OrgPositions.AsQueryable();

            if (departmentId != null)
            {
                query = query.Where(e => e.OrgUnit != null && (e.OrgUnit.Id == departmentId || e.OrgUnit.ParentOrgUnitId == departmentId));
            }

            var results = await query
                .Select(e => new OrgPosition
                {
                    Id = e.Id,
                    PositionCode = e.PositionCode,
                    Name = e.Name,
                    OrgUnitId = e.OrgUnitId,
                    ParentOrgPositionId = e.ParentOrgPositionId,
                    OrgUnit = e.OrgUnit == null ? null : new Domain.Entities.OrgUnit
                    {
                        Id = e.OrgUnit.Id,
                        Name = e.OrgUnit.Name,
                        UnitId = e.OrgUnit.UnitId,
                        ParentOrgUnit = e.OrgUnit.ParentOrgUnit != null && e.OrgUnit.ParentOrgUnit.UnitId == (int)UnitEnum.Department ? new Domain.Entities.OrgUnit
                        {
                            Id = e.OrgUnit.ParentOrgUnit.Id,
                            Name = e.OrgUnit.ParentOrgUnit.Name,
                            UnitId = e.OrgUnit.ParentOrgUnit.UnitId
                        } : null
                    },
                    ParentOrgPosition = e.ParentOrgPosition == null ? null : new OrgPosition
                    {
                        Id = e.ParentOrgPosition.Id,
                        PositionCode = e.ParentOrgPosition.PositionCode,
                        Name = e.ParentOrgPosition.Name,
                        OrgUnitId = e.ParentOrgPosition.OrgUnitId,
                        ParentOrgPositionId = e.ParentOrgPosition.ParentOrgPositionId,
                    }
                })
                .ToListAsync();

            return results;
        }
    }
}