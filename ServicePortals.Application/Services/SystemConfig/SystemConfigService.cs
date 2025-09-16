using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.SystemConfig;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Shared.Exceptions;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Services.SystemConfig
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly ApplicationDbContext _context;

        public SystemConfigService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// lấy config theo key
        /// 
        /// </summary>
        public async Task<Entities.SystemConfig?> GetByConfigKey(string configkey)
        {
            var result = await _context.SystemConfigs.FirstOrDefaultAsync(e => e.ConfigKey == configkey);

            return result;
        }

        /// <summary>
        /// 
        /// thêm mới config của hệ thống, vdu như giới hạn upload file memo, email hr,...
        /// 
        /// </summary>
        public async Task<Entities.SystemConfig> AddConfig(Entities.SystemConfig request)
        {
            var newConfig = new Domain.Entities.SystemConfig
            {
                ConfigKey = request.ConfigKey,
                ConfigValue = request.ConfigValue,
                ValueType = request.ValueType,
                DefaultValue = request.DefaultValue,
                MinValue = request.MinValue,
                MaxValue = request.MaxValue,
                Description = request.Description,
                IsActive = true,
                UpdatedBy = request.UpdatedBy,
                UpdatedAt = DateTimeOffset.Now
            };

            _context.SystemConfigs.Add(newConfig);

            await _context.SaveChangesAsync();

            return newConfig;
        }

        /// <summary>
        /// 
        /// cập nhật config
        /// 
        /// </summary>
        public async Task<Entities.SystemConfig> UpdateConfig(string configkey, Entities.SystemConfig request)
        {
            var config = await _context.SystemConfigs.FirstOrDefaultAsync(e => e.ConfigKey == configkey) ?? throw new NotFoundException("Config not found!");

            //config.Id = config.Id;
            //config.ConfigKey = request.ConfigKey;
            config.ConfigValue = request.ConfigValue;
            //config.ValueType = request.ValueType;
            //config.DefaultValue = request.DefaultValue;
            //config.MinValue = request.MinValue;
            //config.MaxValue = request.MaxValue;
            //config.Description = request.Description;
            //config.IsActive = 1;
            //config.UpdatedBy = request.UpdatedBy;
            config.UpdatedAt = DateTimeOffset.Now;

            _context.SystemConfigs.Update(config);

            await _context.SaveChangesAsync();

            return config;
        }

        public async Task<List<Entities.SystemConfig>> GetAll()
        {
            var result = await _context.SystemConfigs.ToListAsync();

            return result;
        }
    }
}
