using ServicePortals.Application.Dtos.OrgUnit;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;

namespace ServicePortals.Application.Services.OrgUnit
{
    public class OrgUnitService : IOrgUnitService
    {
        private readonly IViclockDapperContext _viclockDapperContext;

        public OrgUnitService (IViclockDapperContext viclockDapperContext)
        {
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

        public async Task<dynamic?> GetAllDepartmentInOrgUnit()
        {
            string sql = $@"SELECT Id, DeptId, Name FROM [{Global.DbViClock}].[dbo].OrgUnits WHERE UnitId = @Id";

            var param = new
            {
                Id = 3, //3 là bộ phận
            };

            var data = await _viclockDapperContext.QueryAsync<dynamic>(sql, param);

            return data;
        }

        public async Task<dynamic?> GetOrgUnitByDept(int deptId)
        {
            string sql = $@"SELECT Id, DeptId, Name FROM [{Global.DbViClock}].[dbo].OrgUnits WHERE UnitId = @UnitId AND ParentOrgUnitId = @deptId";

            var param = new
            {
                UnitId = 4,
                deptId,
            };

            var data = await _viclockDapperContext.QueryAsync<dynamic>(sql, param);

            return data;
        }
    }
}
