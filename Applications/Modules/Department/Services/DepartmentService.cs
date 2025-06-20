using Microsoft.EntityFrameworkCore;
using ServicePortal.Applications.Modules.Department.DTO.Responses;
using ServicePortal.Applications.Modules.Department.Services.Interfaces;
using ServicePortal.Common.Helpers;
using ServicePortal.Infrastructure.Cache;
using ServicePortal.Infrastructure.Data;

namespace ServicePortal.Applications.Modules.Department.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ViclockDbContext _viclockDbContext;
        private readonly ICacheService _cacheService;

        public DepartmentService(ViclockDbContext viclockDbContext, ICacheService cacheService)
        {
            _viclockDbContext = viclockDbContext;
            _cacheService = cacheService;
        }

        public async Task<List<GetAllDepartmentResponse>> GetAll()
        {
            var items = await _cacheService.GetOrCreateAsync(Global.CacheKeyGetAllDepartment, async () =>
            {
                FormattableString sql = $"SELECT BPMa, BPTen FROM tblBoPhan ORDER BY BPMa ASC";

                return await _viclockDbContext
                     .Database
                     .SqlQuery<GetAllDepartmentResponse>(sql)
                     .ToListAsync();

            }, expireMinutes: 1440);

            return items;
        }
    }
}
