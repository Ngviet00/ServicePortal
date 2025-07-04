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
    }
}
