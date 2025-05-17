using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.UserConfig.Services.Interfaces;

namespace ServicePortal.Modules.UserConfig.Services
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
            return await _context.UserConfigs.FirstOrDefaultAsync(e => e.UserCode == userCode && e.ConfigKey == key) ?? null;
        }

        public async Task<Domain.Entities.UserConfig> SaveOrUpdate(Domain.Entities.UserConfig request)
        {
            var config = await _context.UserConfigs.Where(e => e.ConfigKey == request.ConfigKey && e.UserCode == request.UserCode).FirstOrDefaultAsync();

            if (config == null)
            {
                var newConfig = new Domain.Entities.UserConfig
                {
                    UserCode = request.UserCode,
                    ConfigKey = request.ConfigKey,
                    ConfigValue = request.ConfigValue,
                    CreatedAt = DateTimeOffset.Now,
                };

                _context.UserConfigs.Add(newConfig);

                await _context.SaveChangesAsync();

                return newConfig;
            }

            config.ConfigValue = request.ConfigValue;
            _context.UserConfigs.Update(config);

            await _context.SaveChangesAsync();
            return config;
        }
    }
}
