using System.Data;
using ServicePortal.Applications.Viclock.DTO.User;
using ServicePortal.Infrastructure.Cache;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Persistence;

namespace ServicePortal.Applications.Viclock.Queries
{
    public interface IViclockUserQuery
    {
        Task<GetUserPersonalInfoResponseDto?> GetMe(string userCode);
        Task<bool> CheckUserIsExistsInViClock(string UserCode);
    }

    public class ViclockUserQuery : IViclockUserQuery
    {
        private readonly ViclockDbContext _viclockDbContext;

        private readonly IDapperQueryService _dapperQueryService;

        private readonly ICacheService _cacheService;

        public ViclockUserQuery( ViclockDbContext viclockDbContext, IDapperQueryService dapperQueryService, ICacheService cacheService)
        {
            _viclockDbContext = viclockDbContext;
            _dapperQueryService = dapperQueryService;
            _cacheService = cacheService;
        }

        public async Task<GetUserPersonalInfoResponseDto?> GetMe(string UserCode)
        {
            var result = await _cacheService.GetOrCreateAsync($"user_info_{UserCode}", async () =>
            {
                return await _dapperQueryService.QueryFirstOrDefaultAsync<GetUserPersonalInfoResponseDto>(
                    "dbo.usp_GetNhanVienByUserCode",
                    new { UserCode },
                    CommandType.StoredProcedure
                );
            }, expireMinutes: 30);

            return result;
        }

        public async Task<bool> CheckUserIsExistsInViClock(string UserCode)
        {
            var result = await _dapperQueryService.QueryFirstOrDefaultAsync<int>("SELECT 1 FROM tblNhanVien WHERE NVMaNV = @UserCode", new { UserCode });

            return result == 1;
        }
    }
}
