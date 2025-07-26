using Microsoft.Data.SqlClient;
using System.Data;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Application.Services.OrgUnit
{
    public class OrgUnitService : IOrgUnitService
    {
        private readonly ApplicationDbContext _context;
        private readonly IViclockDapperContext _viclockDapperContext;

        public OrgUnitService (ApplicationDbContext context, IViclockDapperContext viclockDapperContext)
        {
            _context = context;
            _viclockDapperContext = viclockDapperContext;
        }

        //lấy orgUnit theo id
        public async Task<Domain.Entities.OrgUnit?> GetOrgUnitById(int id)
        {
            var result = await _context.OrgUnits.FirstOrDefaultAsync(e => e.Id == id);
            
            return result;
        }

        //lấy tất cả phòng ban và vị trị thuộc phòng ban đó
        public async Task<dynamic?> GetAllDepartmentAndFirstOrgUnit()
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"
                SELECT 
                    pb.Id AS DepartmentId,
                    pb.DeptId,
                    pb.Name AS DepartmentName,
                    org.Id AS OrgUnitId,
                    org.Name AS OrgUnitName
                FROM [{Global.DbWeb}].dbo.org_units pb
                LEFT JOIN [{Global.DbWeb}].dbo.org_units org
                    ON org.ParentOrgUnitId = pb.Id
                WHERE pb.DeptId IS NOT NULL AND org.Id IS NOT NULL
                ORDER BY pb.Id, org.Id
            ";

            Console.WriteLine(sql.ToString());

            var rawData = await connection.QueryAsync<dynamic>(sql);

            var result = rawData
                .GroupBy(x => new { x.DepartmentId, x.DepartmentName })
                .Select(group => new
                {
                    id = group.Key.DepartmentId.ToString(),
                    label = group.Key.DepartmentName,
                    type = "department",
                    children = group.Select(x => new
                    {
                        id = x.OrgUnitId?.ToString() ?? "",
                        label = x.OrgUnitName ?? "",
                        type = "jobtitle"
                    }).ToList()
                })
                .ToList();

            return result;
        }
    }
}
