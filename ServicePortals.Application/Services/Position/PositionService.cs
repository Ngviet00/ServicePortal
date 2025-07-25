using ServicePortal.Infrastructure.Cache;
using ServicePortals.Application.Dtos.Position.Responses;
using ServicePortals.Application.Interfaces.Position;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;

namespace ServicePortals.Infrastructure.Services.Position
{
    public class PositionService : IPositionService
    {
        private readonly IViclockDapperContext _viclockDapperContext;
        private readonly ICacheService _cacheService;

        public PositionService(IViclockDapperContext viclockDbContext, ICacheService cacheService)
        {
            _viclockDapperContext = viclockDbContext;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 
        /// Lấy tất cả vị trí từ db viclock
        /// 
        /// </summary>
        public async Task<List<GetAllPositionResponse>> GetAll()
        {
            var items = await _cacheService.GetOrCreateAsync(Global.CacheKeyGetAllPosition, async () =>
            {
                var sql = $@"SELECT CVMa, CVTen FROM tblChucVu ORDER BY CVTen ASC";

                return await _viclockDapperContext.QueryAsync<GetAllPositionResponse>(sql);

            }, expireMinutes: 1440);

            return (List<GetAllPositionResponse>)items;
        }
    }
}