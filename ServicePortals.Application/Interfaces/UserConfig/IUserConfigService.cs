﻿namespace ServicePortals.Application.Interfaces.UserConfig
{
    public interface IUserConfigService
    {
        Task<Domain.Entities.UserConfig?> GetConfigByUserCodeAndkey(string userCode, string key);
        Task<Domain.Entities.UserConfig> SaveOrUpdate(Domain.Entities.UserConfig request);
    }
}
