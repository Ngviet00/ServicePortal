using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServicePortals.Application.Interfaces;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services
{
    public class CommonDataService : ICommonDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public CommonDataService (ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<RequestStatus>?> GetAllRequestStatus()
        {
            var requestStatus = await _cache.GetOrCreateAsync("request_status", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                return await _context.RequestStatuses.ToListAsync();
            });

            return requestStatus;
        }

        public async Task<List<TimeLeave>?> GetAllTimeLeave()
        {
            var timeLeave = await _cache.GetOrCreateAsync($"time_leaves", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                return await _context.TimeLeaves.ToListAsync();
            });

            return timeLeave;
        }
    }
}
