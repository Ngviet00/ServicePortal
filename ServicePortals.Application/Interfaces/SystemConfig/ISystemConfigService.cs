using Entities = ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.SystemConfig
{
    public interface ISystemConfigService
    {
        Task<List<Entities.SystemConfig>> GetAll();
        Task<Entities.SystemConfig?> GetByConfigKey(string configkey);
        Task<Entities.SystemConfig> AddConfig(Entities.SystemConfig request);
        Task<Entities.SystemConfig> UpdateConfig(string configkey, Entities.SystemConfig request);
    }
}
