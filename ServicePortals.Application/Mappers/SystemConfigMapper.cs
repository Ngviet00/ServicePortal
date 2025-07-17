using ServicePortals.Application.Dtos.SystemConfig;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Mappers
{
    public static class SystemConfigMapper
    {
        public static SystemConfigDto? ToDto(SystemConfig? entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new SystemConfigDto
            {
                Id = entity.Id,
                ConfigKey = entity.ConfigKey,
                ConfigValue = entity.ConfigValue,
                ValueType = entity.ValueType,
                DefaultValue = entity.DefaultValue,
                MinValue = entity.MinValue,
                MaxValue = entity.MaxValue,
                Description = entity.Description,
                IsActive = entity.IsActive,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static List<SystemConfigDto?> ToDtoList(List<SystemConfig> systemConfigs)
        {
            return systemConfigs.Select(ToDto).ToList();
        }
    }
}
