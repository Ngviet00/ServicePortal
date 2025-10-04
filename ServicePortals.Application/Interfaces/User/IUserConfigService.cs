namespace ServicePortals.Application.Interfaces.User
{
    public interface IUserConfigService
    {
        Task<Domain.Entities.UserConfig?> GetConfigByUserCodeAndkey(string userCode, string key);
        Task<Domain.Entities.UserConfig> SaveOrUpdate(Domain.Entities.UserConfig request);
    }
}
