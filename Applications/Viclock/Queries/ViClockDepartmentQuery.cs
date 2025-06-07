using Microsoft.EntityFrameworkCore;
using ServicePortal.Applications.Viclock.DTO.Department;
using ServicePortal.Infrastructure.Cache;
using ServicePortal.Infrastructure.Data;

namespace ServicePortal.Applications.Viclock.Queries
{
    public interface IViClockDepartmentQuery
    {
        Task<List<DepartmentViclockDtos>> GetAll();
    }

    public class ViClockDepartmentQuery : IViClockDepartmentQuery
    {
        private readonly ViclockDbContext _viclockDbContext;

        private readonly ICacheService _cacheService;

        private const string CacheKeyGetAllDepartment = "get_all_department";

        public ViClockDepartmentQuery(ViclockDbContext viclockDbContext, ICacheService cacheService)
        {
            _viclockDbContext = viclockDbContext;
            _cacheService = cacheService;
        }

        public async Task<List<DepartmentViclockDtos>> GetAll()
        {
            var items = await _cacheService.GetOrCreateAsync(CacheKeyGetAllDepartment, async () =>
            {
                FormattableString sql = $"SELECT BPMa, BPTen FROM tblBoPhan";

                return await _viclockDbContext
                     .Database
                     .SqlQuery<DepartmentViclockDtos>(sql)
                     .ToListAsync();

            }, expireMinutes: 1440);

            return items;
        }
    }
}
