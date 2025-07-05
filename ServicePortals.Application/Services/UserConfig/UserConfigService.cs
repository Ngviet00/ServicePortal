using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.UserConfig;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Infrastructure.Services.UserConfig
{
    public class UserConfigService : IUserConfigService
    {
        private readonly ApplicationDbContext _context;
        public UserConfigService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.UserConfig?> GetConfigByUserCodeAndkey(string userCode, string key)
        {
            return await _context.UserConfigs.FirstOrDefaultAsync(e => e.UserCode == userCode && e.Key == key) ?? null;
        }

        public async Task<Domain.Entities.UserConfig> SaveOrUpdate(Domain.Entities.UserConfig request)
        {
            var config = await _context.UserConfigs.Where(e => e.Key == request.Key && e.UserCode == request.UserCode).FirstOrDefaultAsync();

            if (config == null)
            {
                var newConfig = new Domain.Entities.UserConfig
                {
                    UserCode = request.UserCode,
                    Key = request.Key,
                    Value = request.Value
                };

                _context.UserConfigs.Add(newConfig);
                await _context.SaveChangesAsync();

                return newConfig;
            }

            config.Value = request.Value;

            _context.UserConfigs.Update(config);
            await _context.SaveChangesAsync();

            return config;
        }
    }
}
