using ServicePortals.Application.Dtos.SystemConfig;

namespace ServicePortals.Application.Interfaces.SystemConfig
{
    public interface ISystemConfigService
    {
        Task<List<SystemConfigDto?>> GetAll();
        Task<SystemConfigDto?> GetByConfigKey(string configkey);
        Task<SystemConfigDto?> AddConfig(SystemConfigDto request);
        Task<SystemConfigDto?> UpdateConfig(string configkey, SystemConfigDto request);
    }
}
