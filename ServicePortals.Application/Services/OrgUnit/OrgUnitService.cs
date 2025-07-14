using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using Dapper;

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

        public async Task<dynamic?> GetOrgUnitById(int id)
        {
            string sql = $@"SELECT * FROM [{Global.DbViClock}].[dbo].OrgUnits WHERE ID = @Id";

            var param = new
            {
                Id = id,
            };

            var data = await _viclockDapperContext.QueryFirstOrDefaultAsync<dynamic>(sql, param);

            return data;
        }

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
                FROM [{Global.DbViClock}].dbo.OrgUnits pb
                LEFT JOIN [{Global.DbViClock}].dbo.OrgUnits org
                    ON org.ParentOrgUnitId = pb.Id
                WHERE pb.DeptId IS NOT NULL
                ORDER BY pb.Id, org.Id
            ";

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
                        id = x.OrgUnitId.ToString(),
                        label = x.OrgUnitName,
                        type = "jobtitle"
                    }).ToList()
                })
                .ToList();

            return result;
        }
    }
}
