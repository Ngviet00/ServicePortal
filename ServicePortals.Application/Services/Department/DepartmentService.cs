using ServicePortal.Infrastructure.Cache;
using ServicePortals.Application.Dtos.Department.Responses;
using ServicePortals.Application.Interfaces.Department;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;

namespace ServicePortals.Infrastructure.Services.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IViclockDapperContext _viclockDapperContext;
        private readonly ICacheService _cacheService;

        public DepartmentService(IViclockDapperContext viclockDapperContext, ICacheService cacheService)
        {
            _viclockDapperContext = viclockDapperContext;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 
        /// Lấy tất cả phòng ban bên db viclock
        /// 
        /// </summary>
        public async Task<List<GetAllDepartmentResponse>> GetAll()
        {
            var items = await _cacheService.GetOrCreateAsync(Global.CacheKeyGetAllDepartment, async () =>
            {
                var sql = $@"SELECT BPMa, BPTen FROM tblBoPhan ORDER BY BPMa ASC";

                return await _viclockDapperContext.QueryAsync<GetAllDepartmentResponse>(sql);

            }, expireMinutes: 1440);

            return (List<GetAllDepartmentResponse>)items;
        }

        /// <summary>
        /// 
        /// Lấy tất cả phòng ban bên db viclock loại bỏ trùng tên, chỉ lấy tên
        /// 
        /// </summary>
        public async Task<List<string>> GetAllWithDistinctName()
        {
            var items = await _cacheService.GetOrCreateAsync(Global.CacheKeyGetAllDepartmentDistinctName, async () =>
            {
                var sql = $@"SELECT DISTINCT BpTen FROM tblBoPhan ORDER BY BPTen";

                return (List<string>)await _viclockDapperContext.QueryAsync<string>(sql);

            }, expireMinutes: 1440);

            return items;
        }
    }
}