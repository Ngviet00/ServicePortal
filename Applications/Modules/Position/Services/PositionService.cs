using Microsoft.EntityFrameworkCore;
using ServicePortal.Applications.Modules.Position.DTO.Responses;
using ServicePortal.Applications.Modules.Position.Services.Interfaces;
using ServicePortal.Common.Helpers;
using ServicePortal.Infrastructure.Cache;
using ServicePortal.Infrastructure.Data;

namespace ServicePortal.Applications.Modules.Position.Services
{
    public class PositionService : IPositionService
    {
        private readonly ViclockDbContext _viclockDbContext;

        private readonly ICacheService _cacheService;

        public PositionService(ViclockDbContext viclockDbContext, ICacheService cacheService)
        {
            _viclockDbContext = viclockDbContext;
            _cacheService = cacheService;
        }

        public async Task<List<GetAllPositionResponse>> GetAll()
        {
            var items = await _cacheService.GetOrCreateAsync(Global.CacheKeyGetAllPosition, async () =>
            {
                FormattableString sql = $"SELECT CVMa, CVTen FROM tblChucVu ORDER BY CVTen ASC";

                return await _viclockDbContext
                     .Database
                     .SqlQuery<GetAllPositionResponse>(sql)
                     .ToListAsync();

            }, expireMinutes: 1440);

            return items;
        }
    }
}
